using Application.Common.Extensions;
using Application.DTOs.WalletDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ICurrentUserService _currentUserService;

        private readonly IWalletRepository _walletRepository;

        private readonly IWalletTransactionRepository _walletTransactionRepository;

        private readonly ILogger<WalletService> _logger;

        private readonly IValidator<DepositRequest> _depositValidator;

        private readonly ITransactionReadRepository _transactionReadRepository;

        public WalletService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            ILogger<WalletService> logger,
            IValidator<DepositRequest> depositValidator,
            ITransactionReadRepository transactionReadRepository
            )
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _logger = logger;
            _depositValidator  = depositValidator;
            _transactionReadRepository = transactionReadRepository;
        }

        public async Task<Result<WalletTransactionResponse>> DepositToWallet(DepositRequest request)
        {
            var userId = CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Starting wallet deposit for User {UserId} with Amount {Amount}.",
                userId,
                request.Amount);

            var validationResult =
                await _depositValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Invalid wallet deposit request for User {UserId}.",
                    userId);

                return Result<WalletTransactionResponse>.Failure(
                    validationResult.Errors.First().ErrorMessage);
            }

            var userWallet =
                await _walletRepository.GetByIdAsync(userId);

            if (userWallet is null)
            {
                _logger.LogWarning(
                    "Wallet for User {UserId} was not found.",
                    userId);

                return Result<WalletTransactionResponse>.Failure(
                    "Wallet does not exist.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                userWallet.Deposit(request.Amount);

                var transaction = new WalletTransaction(
                    userWallet.Id,
                    request.Amount,
                    Domain.Enums.TransactionType.Deposit,
                    request.Description,
                    null);

                await _walletTransactionRepository.AddAsync(transaction);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                var transactionResponse = new WalletTransactionResponse
                {
                    Amount = transaction.Amount,
                    BalanceAfter = userWallet.Balance,
                    createdAt = transaction.CreatedAt,
                    Description = transaction.Description,
                    Type = transaction.Type,
                    WalletId = userWallet.Id
                };

                _logger.LogInformation(
                    "Wallet deposit completed successfully for User {UserId}. " +
                    "Wallet {WalletId}, Transaction {TransactionId}, Amount {Amount}.",
                    userId,
                    userWallet.Id,
                    transaction.Id,
                    transaction.Amount);

                return Result<WalletTransactionResponse>.Success(
                    transactionResponse);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "An error occurred while depositing {Amount} to Wallet {WalletId} for User {UserId}.",
                    request.Amount,
                    userWallet.Id,
                    userId);

                throw;
            }
        }

        public async Task<Result<WalletTransactionResponse>> GetTransactionById(Guid transactionId)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting wallet transaction {TransactionId} for User {UserId}.",
                transactionId,
                userId);

            var transaction =
                await _walletTransactionRepository.GetByIdAsync(transactionId);

            if (transaction is null)
            {
                _logger.LogWarning(
                    "Wallet transaction {TransactionId} was not found for User {UserId}.",
                    transactionId,
                    userId);

                return Result<WalletTransactionResponse>.Failure(
                    "Transaction does not exist.");
            }

            if (transaction.Wallet.UserId != userId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access Wallet Transaction {TransactionId} without permission.",
                    userId,
                    transactionId);

                return Result<WalletTransactionResponse>.Failure(
                    "Access denied.");
            }

            var transactionDto = new WalletTransactionResponse
            {
                TransactionId = transaction.Id,
                Amount = transaction.Amount,
                BalanceAfter = transaction.Wallet.Balance,
                createdAt = transaction.CreatedAt,
                Description = transaction.Description,
                Type = transaction.Type,
                WalletId = transaction.WalletId,
                OrderId = transaction.OrderId
            };

            _logger.LogInformation(
                "Wallet transaction {TransactionId} retrieved successfully for User {UserId}.",
                transactionId,
                userId);

            return Result<WalletTransactionResponse>.Success(transactionDto);
        }

        public async Task<Result<PaginatedResult<WalletTransactionResponse>>> GetTransactions(WalletPaginationRequest request)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting wallet transactions for User {UserId}. Page {PageNumber}, PageSize {PageSize}.",
                userId,
                request.PageNumber,
                request.PageSize);

            var response =
                await _transactionReadRepository.ReadPaginationAsync(
                    userId,
                    request);

            _logger.LogInformation(
                "Retrieved wallet transactions for User {UserId}. Page {PageNumber}, PageSize {PageSize}, TotalCount {TotalCount}.",
                userId,
                request.PageNumber,
                request.PageSize,
                response.TotalCount);

            return Result<PaginatedResult<WalletTransactionResponse>>.Success(
                response);
        }

        public async Task<Result<WalletResponse>> GetUserWallet()
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting wallet for User {UserId}.",
                userId);

            var wallet = await _walletRepository.GetWalletByUserId(userId);

            if (wallet is null)
            {
                _logger.LogWarning(
                    "Wallet for User {UserId} was not found.",
                    userId);

                return Result<WalletResponse>.Failure(
                    "Wallet does not exist.");
            }

            var walletResponse = new WalletResponse()
            {
                Balance = wallet.Balance,
                Type = wallet.Type,
                UserId = userId,
                WalletId = wallet.Id,
            };

            _logger.LogInformation(
                "Wallet {WalletId} retrieved successfully for User {UserId}.",
                wallet.Id,
                userId);

            return Result<WalletResponse>.Success(walletResponse);
        }
    }
}
