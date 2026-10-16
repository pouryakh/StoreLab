using Application.DTOs.AuthDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Entities.Auth;
using Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;

        private readonly ITokenService _tokenService;

        private readonly ILogger<AuthService> _logger;

        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private readonly IWalletRepository _walletRepository;

        private readonly IShoppingCartRepository _shoppingCartRepository;

        public AuthService(IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator,
            ILogger<AuthService> logger,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IWalletRepository walletRepository,
            IShoppingCartRepository shoppingCartRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _logger = logger;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _walletRepository = walletRepository;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("User Login Started.");
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("User login failed because the request is invalid.");
                return Result<AuthResponse>.Failure(validationResult.Errors.First().ErrorMessage);
            }
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
            {
                _logger.LogWarning("User With This Email : {Email} Not Founded.",request.Email);
                return Result<AuthResponse>.Failure("InCorrect Password Or Email");   
            }

            // ---- test version  ----------------------
            //if (user.PasswordHash != request.Password)
            //{
            //    _logger.LogWarning("InValid Password For UserId: {UserId} , UserEmail: {UserEmail}.", user.Id, user.Email);
            //    return Result<AuthResponse>.Failure("InCorrect Password Or Email");
            //}

            // -----------------------------------------

            var passwordVerifyResult = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!passwordVerifyResult)
            {
                _logger.LogWarning("InValid Password For UserId: {UserId} , UserEmail: {UserEmail}.", user.Id, user.Email);
                return Result<AuthResponse>.Failure("InCorrect Password Or Email");
            }

            var authResponse = new AuthResponse
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = _tokenService.GenerateRefreshToken()
            };

            var hashedRefreshToken = _tokenService.HashRefreshToken(authResponse.RefreshToken);
            var refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiration();

            var refreshToken = new RefreshToken(user.Id, hashedRefreshToken, refreshTokenExpiresAt);

            await _refreshTokenRepository.AddAsync(refreshToken);

            await _unitOfWork.SaveChangesAsync();

            return Result<AuthResponse>.Success(authResponse);

        }

        public async Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request)
        {
            _logger.LogInformation(
                "Refresh token request started.");

            var hashedToken =
                _tokenService.HashRefreshToken(
                    request.RefreshToken);

            var refreshToken =
                await _refreshTokenRepository
                    .GetByTokenHashAsync(hashedToken);

            if (refreshToken is null)
            {
                _logger.LogWarning(
                    "Refresh token request failed because token was not found.");

                return Result<AuthResponse>.Failure(
                    "Please log in again.");
            }

            if (refreshToken.IsRevoked())
            {
                _logger.LogWarning(
                    "Refresh token request failed because token was revoked. TokenId: {TokenId}",
                    refreshToken.Id);

                return Result<AuthResponse>.Failure(
                    "Please log in again.");
            }

            if (refreshToken.IsExpired())
            {
                _logger.LogWarning(
                    "Refresh token request failed because token was expired. TokenId: {TokenId}",
                    refreshToken.Id);

                return Result<AuthResponse>.Failure(
                    "Please log in again.");
            }

            var user =
                await _userRepository.GetByIdAsync(
                    refreshToken.UserId);

            if (user is null)
            {
                _logger.LogWarning(
                    "Refresh token request failed because user was not found. UserId: {UserId}",
                    refreshToken.UserId);

                return Result<AuthResponse>.Failure(
                    "Please log in again.");
            }

            var newRefreshToken =
                _tokenService.GenerateRefreshToken();

            var newHashedToken =
                _tokenService.HashRefreshToken(
                    newRefreshToken);

            var newExpiresAt =
                _tokenService.GetRefreshTokenExpiration();

            var newRefreshTokenEntity =
                new RefreshToken(
                    user.Id,
                    newHashedToken,
                    newExpiresAt);

            refreshToken.Revoke();

            _refreshTokenRepository.Update(
                refreshToken);

            await _refreshTokenRepository.AddAsync(
                newRefreshTokenEntity);

            await _unitOfWork.SaveChangesAsync();

            var authResponse = new AuthResponse
            {
                AccessToken =
                    _tokenService.GenerateAccessToken(user),

                RefreshToken = newRefreshToken
            };

            _logger.LogInformation(
                "Refresh token rotation completed successfully. UserId: {UserId}, OldTokenId: {OldTokenId}, NewTokenId: {NewTokenId}",
                user.Id,
                refreshToken.Id,
                newRefreshTokenEntity.Id);

            return Result<AuthResponse>.Success(authResponse);
        }

        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            _logger.LogInformation("User registration started.");
            var validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("User registration failed because the request is invalid.");
                return Result<RegisterResponse>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            if (await _userRepository.GetByEmailAsync(request.Email) != null)
            {
                _logger.LogWarning("User registration failed because email -{Email}- already exists.",request.Email);
                return Result<RegisterResponse>.Failure("Email already exists.");
            }

            if (await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber) != null)
            {
                _logger.LogWarning("User registration failed because phoneNumber -{PhoneNumber}- already exists.",request.PhoneNumber);
                return Result<RegisterResponse>.Failure("PhoneNumber already exists.");
            }

            var hashPassword = _passwordHasher.Hash(request.Password);

            var user = new User(request.FirstName, request.LastName, request.Email, hashPassword, request.PhoneNumber,Domain.Enums.UserRole.Buyer);

            await _userRepository.AddAsync(user);

            var wallet = new Wallet(user.Id, WalletType.Buyer);

            await _walletRepository.AddAsync(wallet);

            var shoppingCart = new ShoppingCart(user.Id);

            await _shoppingCartRepository.AddAsync(shoppingCart);

            await _unitOfWork.SaveChangesAsync();

            RegisterResponse response = new RegisterResponse();
            response.FirstName = request.FirstName;
            response.LastName = request.LastName;
            response.UserId = user.Id;
            response.Email = request.Email;

            _logger.LogInformation("User registered successfully. UserId: {UserId}",user.Id);
            return Result<RegisterResponse>.Success(response);
        }
    }
}
