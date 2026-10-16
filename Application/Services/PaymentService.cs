using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IWalletRepository _walletRepository;

        private readonly IWalletTransactionRepository _walletTransactionRepository;

        private readonly IPaymentRepository _paymentRepository;

        private readonly ILogger<PaymentService> _logger;

        private readonly ICommissionService _commissionService;

        public PaymentService(
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IPaymentRepository paymentRepository,
            ICommissionService commissionService,
            ILogger<PaymentService> logger
            )
        {
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _commissionService = commissionService;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<Result<Wallet>> PayOrderAsync(Order order)
        {
            _logger.LogInformation(
                "Payment for Order {OrderId} started. Amount: {Amount}, UserId: {UserId}",
                order.Id,
                order.TotalAmount,
                order.UserId);

            var payment = new Payment(order.Id, order.TotalAmount);

            await _paymentRepository.AddAsync(payment);

            var userWallet = await _walletRepository.GetWalletByUserId(order.UserId);

            if (userWallet is null)
            {
                _logger.LogError(
                    "Buyer wallet not found for User {UserId} while paying Order {OrderId}.",
                    order.UserId,
                    order.Id);

                throw new InvalidOperationException(
                    $"Wallet not found for user: {order.UserId}");
            }

            var escrowWallet = await _walletRepository.GetEscrowWalletAsync();

            if (escrowWallet is null)
            {
                _logger.LogError(
                    "Escrow wallet not found while paying Order {OrderId}.",
                    order.Id);

                throw new InvalidOperationException("Escrow Wallet not found");
            }

            _logger.LogInformation("order TotalAmount {orderTotalAmount} --- WalletBalance {WalletBalance}", order.TotalAmount,userWallet.Balance);
            userWallet.Withdraw(order.TotalAmount);
            escrowWallet.Deposit(order.TotalAmount);

            var walletTransaction = new WalletTransaction(
                userWallet.Id,
                order.TotalAmount,
                Domain.Enums.TransactionType.Payment,
                $"Payed order {order.Id} with amount {order.TotalAmount}",
                order.Id);

            await _walletTransactionRepository.AddAsync(walletTransaction);

            payment.MarkAsSuccessful();

            _logger.LogInformation(
                "Payment for Order {OrderId} completed successfully. Amount: {Amount}.",
                order.Id,
                order.TotalAmount);

            return Result<Wallet>.Success(userWallet);
        }

        public async Task<SimpleResult> RefundCancelOrderAsync(StoreOrder storeOrder, Guid userId)
        {
            _logger.LogInformation(
                "Starting refund for cancelled StoreOrder {StoreOrderId}, UserId {UserId}, Amount {RefundAmount}.",
                storeOrder.Id,
                userId,
                storeOrder.TotalAmount);

            try
            {
                var escrowWallet = await _walletRepository.GetEscrowWalletAsync();

                if (escrowWallet is null)
                {
                    _logger.LogError(
                        "Escrow wallet not found while refunding StoreOrder {StoreOrderId}.",
                        storeOrder.Id);

                    throw new InvalidOperationException("Escrow Wallet not found.");
                }

                var userWallet = await _walletRepository.GetWalletByUserId(userId);

                if (userWallet is null)
                {
                    _logger.LogError(
                        "User wallet not found while refunding StoreOrder {StoreOrderId}, UserId {UserId}.",
                        storeOrder.Id,
                        userId);

                    throw new InvalidOperationException("User Wallet not found.");
                }

                var amount = storeOrder.TotalAmount;

                _logger.LogInformation(
                    "Starting refund for StoreOrder {StoreOrderId}, UserId {UserId}, Amount {RefundAmount}.",
                    storeOrder.Id,
                    userId,
                    amount);

                escrowWallet.Withdraw(amount);
                userWallet.Deposit(amount);

                var escrowTransaction = new WalletTransaction(
                    escrowWallet.Id,
                    amount,
                    TransactionType.Withdrawal,
                    $"Escrow refund withdrawal for StoreOrder {storeOrder.Id}.",
                    storeOrder.OrderId);

                var buyerTransaction = new WalletTransaction(
                    userWallet.Id,
                    amount,
                    TransactionType.Deposit,
                    $"Refund for StoreOrder {storeOrder.Id}. TotalAmount: {amount}.",
                    storeOrder.OrderId);

                await _walletTransactionRepository.AddAsync(escrowTransaction);
                await _walletTransactionRepository.AddAsync(buyerTransaction);

                _logger.LogInformation(
                    "Refund transactions created successfully for StoreOrder {StoreOrderId}. Amount: {RefundAmount}, EscrowTransactionId: {EscrowTransactionId}, BuyerTransactionId: {BuyerTransactionId}.",
                    storeOrder.Id,
                    amount,
                    escrowTransaction.Id,
                    buyerTransaction.Id);

                return SimpleResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Refund failed for StoreOrder {StoreOrderId}, UserId {UserId}.",
                    storeOrder.Id,
                    userId);

                throw;
            }
        }

        public async Task<SimpleResult> RefundReturnRequestAsync(ReturnRequest returnRequest)
        {
            _logger.LogInformation(
                "Starting refund for ReturnRequest {ReturnRequestId}, StoreOrder {StoreOrderId}.",
                returnRequest.Id,
                returnRequest.StoreOrderId);

            try
            {
                var escrowWallet = await _walletRepository.GetEscrowWalletAsync();

                if (escrowWallet is null)
                {
                    _logger.LogError(
                        "Escrow wallet not found while refunding StoreOrder {StoreOrderId}.",
                        returnRequest.StoreOrderId);

                    throw new InvalidOperationException("Escrow Wallet not found.");
                }

                var userWallet = await _walletRepository.GetWalletByUserId(
                    returnRequest.UserId);

                if (userWallet is null)
                {
                    _logger.LogError(
                        "User wallet not found while refunding StoreOrder {StoreOrderId}, UserId {UserId}.",
                        returnRequest.StoreOrderId,
                        returnRequest.UserId);

                    throw new InvalidOperationException("User Wallet not found.");
                }


                var amount = returnRequest.StoreOrder.TotalAmount;

                _logger.LogInformation(
                    "Processing refund of {RefundAmount} for StoreOrder {StoreOrderId}.",
                    amount,
                    returnRequest.StoreOrderId);

                escrowWallet.Withdraw(amount);
                userWallet.Deposit(amount);

                var escrowTransaction = new WalletTransaction(
                    escrowWallet.Id,
                    amount,
                    TransactionType.Withdrawal,
                    $"Escrow refund withdrawal for StoreOrder {returnRequest.StoreOrder.Id}.",
                    returnRequest.StoreOrder.OrderId,
                    returnRequest.Id);

                var buyerTransaction = new WalletTransaction(
                    userWallet.Id,
                    amount,
                    TransactionType.Refund,
                    $"Refund for StoreOrder {returnRequest.StoreOrder.Id}. TotalAmount: {amount}.",
                    returnRequest.StoreOrder.OrderId,
                    returnRequest.Id);

                await _walletTransactionRepository.AddAsync(escrowTransaction);
                await _walletTransactionRepository.AddAsync(buyerTransaction);

                _logger.LogInformation(
                    "Refund transaction created successfully. ReturnRequestId: {ReturnRequestId}, StoreOrderId: {StoreOrderId}, Amount: {RefundAmount}, TransactionId: {TransactionId}.",
                    returnRequest.Id,
                    returnRequest.StoreOrderId,
                    amount,
                    buyerTransaction.Id);

                return SimpleResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Refund failed for ReturnRequest {ReturnRequestId}, StoreOrder {StoreOrderId}, UserId {UserId}.",
                    returnRequest.Id,
                    returnRequest.StoreOrderId,
                    returnRequest.UserId);

                throw;
            }
        }

        public async Task<SimpleResult> SettleStoreOrderAsync(StoreOrder storeOrder)
        {
            _logger.LogInformation(
                "Settlement for StoreOrder {StoreOrderId} started. StoreId: {StoreId}, Amount: {Amount}.",
                storeOrder.Id,
                storeOrder.StoreId,
                storeOrder.TotalAmount);

            var escrowWallet = await _walletRepository.GetEscrowWalletAsync();

            if (escrowWallet is null)
            {
                _logger.LogError(
                    "Escrow wallet not found while settling StoreOrder {StoreOrderId}.",
                    storeOrder.Id);

                throw new InvalidOperationException("Escrow Wallet not found.");
            }

            var platformWallet = await _walletRepository.GetPlatformWalletAsync();

            if (platformWallet is null)
            {
                _logger.LogError(
                    "Platform wallet not found while settling StoreOrder {StoreOrderId}.",
                    storeOrder.Id);

                throw new InvalidOperationException("Platform Wallet not found.");
            }

            var sellerWallet =
                await _walletRepository.GetSellerWalletByStoreIdAsync(storeOrder.StoreId);

            if (sellerWallet is null)
            {
                _logger.LogError(
                    "Seller wallet not found for Store {StoreId} while settling StoreOrder {StoreOrderId}.",
                    storeOrder.StoreId,
                    storeOrder.Id);

                throw new InvalidOperationException(
                    $"Seller wallet not found for StoreId: {storeOrder.StoreId}");
            }

            var commissionAmount = _commissionService.CalculateCommission(storeOrder.TotalAmount);
            var netAmount = storeOrder.TotalAmount - commissionAmount;
            

            escrowWallet.Withdraw(storeOrder.TotalAmount);
            platformWallet.Deposit(commissionAmount);
            sellerWallet.Deposit(netAmount);

            var sellerDescription =
                $"Settlement for StoreOrder {storeOrder.Id}. " +
                $"TotalAmount: {storeOrder.TotalAmount}, " +
                $"NetAmount: {netAmount}.";

            var sellerTransaction = new WalletTransaction(
                sellerWallet.Id,
                netAmount,
                TransactionType.SellerSettlement,
                sellerDescription);

            await _walletTransactionRepository.AddAsync(sellerTransaction);

            var commissionTransaction = new WalletTransaction(
                platformWallet.Id,
                commissionAmount,
                TransactionType.PlatformCommission,
                $"Platform commission for StoreOrder {storeOrder.Id}. " +
                $"CommissionAmount: {commissionAmount}.");

            await _walletTransactionRepository.AddAsync(commissionTransaction);

            _logger.LogInformation(
                "Settlement for StoreOrder {StoreOrderId} completed. " +
                "TotalAmount: {TotalAmount}, Commission: {CommissionAmount}, NetAmount: {NetAmount}.",
                storeOrder.Id,
                storeOrder.TotalAmount,
                commissionAmount,
                netAmount);

            return SimpleResult.Success();
        }

        
    }
}
