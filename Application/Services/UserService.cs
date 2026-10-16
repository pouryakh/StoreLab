using Application.Common.Extensions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Results;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly ICurrentUserService _currentUserService;

        private readonly IPasswordHasher _passwordHasher;

        private readonly ILogger<UserService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IUserRepository repository,
            ICurrentUserService currentUserService,
            IPasswordHasher passwordHasher,
            ILogger<UserService> logger,
            IUnitOfWork unitOfWork
            )
        {
            _userRepository = repository;
            _currentUserService = currentUserService;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserResponse>> GetUser()
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting profile for User {UserId}.",
                userId);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                _logger.LogWarning(
                    "User {UserId} was not found.",
                    userId);

                return Result<UserResponse>.Failure(
                    "User does not exist.");
            }

            var userResponse = new UserResponse()
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
            };

            _logger.LogInformation(
                "Profile retrieved successfully for User {UserId}.",
                userId);

            return Result<UserResponse>.Success(userResponse);
        }

        public async Task<Result<UserResponse>> UpdateUser(
            UpdateUserRequest request)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Updating profile for User {UserId}.",
                userId);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                _logger.LogWarning(
                    "User {UserId} was not found.",
                    userId);

                return Result<UserResponse>.Failure(
                    "User does not exist.");
            }

            if (request.PhoneNumber != null &&
                await _userRepository.IsPhoneNumberExsists(request.PhoneNumber))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to update to an existing phone number.",
                    userId);

                return Result<UserResponse>.Failure(
                    "Phone number is already in use.");
            }

            if (request.Email != null &&
                await _userRepository.IsEmailExsists(request.Email))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to update to an existing email address.",
                    userId);

                return Result<UserResponse>.Failure(
                    "Email address is already in use.");
            }

            user.UpdateUser(
                request.PhoneNumber,
                request.Email,
                request.FirstName,
                request.LastName);

            await _unitOfWork.SaveChangesAsync();

            var userReponse = new UserResponse()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                UserId = user.Id,
            };

            _logger.LogInformation(
                "Profile updated successfully for User {UserId}.",
                userId);

            return Result<UserResponse>.Success(userReponse);
        }

        public async Task<Result<UserResponse>> UpdateUserPassword(
            UpdatePasswordRequest request)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Updating password for User {UserId}.",
                userId);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                _logger.LogWarning(
                    "User {UserId} was not found.",
                    userId);

                return Result<UserResponse>.Failure(
                    "User does not exist.");
            }

            if (!_passwordHasher.Verify(
                    request.CurrentPassword,
                    user.PasswordHash))
            {
                _logger.LogWarning(
                    "User {UserId} provided an incorrect current password.",
                    userId);

                return Result<UserResponse>.Failure(
                    "Current password is incorrect.");
            }

            user.UpdatePaswword(
                _passwordHasher.Hash(request.NewPassword));

            await _unitOfWork.SaveChangesAsync();

            var userReponse = new UserResponse()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                UserId = user.Id,
            };

            _logger.LogInformation(
                "Password updated successfully for User {UserId}.",
                userId);

            return Result<UserResponse>.Success(userReponse);
        }
    }
}