using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RefundService : IRefundService
    {
        private readonly ILogger<RefundService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IReturnRequestRepository _returnRequestRepository;

        private readonly IPaymentService _paymentService;

        public RefundService(ILogger<RefundService> logger,
            IUnitOfWork unitOfWork,
            IReturnRequestRepository returnRequestRepository,
            IPaymentService paymentService
            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _returnRequestRepository = returnRequestRepository;
            _paymentService = paymentService;
        }


        public async Task<SimpleResult> RefundEligibleOrdersAsync()
        {
            _logger.LogInformation(
                "Starting eligible return requests refund processing.");

            var eligibleReturnRequests =
                await _returnRequestRepository.GetRefundPendingReturnRequestsAsync();

            var totalCount = eligibleReturnRequests.Count;
            var successCount = 0;
            var failedCount = 0;

            _logger.LogInformation(
                "Found {TotalCount} return requests pending refund.",
                totalCount);

            if (totalCount == 0)
            {
                _logger.LogInformation(
                    "No return requests pending refund. Processing finished.");

                return SimpleResult.Success();
            }

            foreach (var returnRequest in eligibleReturnRequests)
            {
                _logger.LogInformation(
                    "Starting refund for ReturnRequestId: {ReturnRequestId}, " +
                    "StoreOrderId: {StoreOrderId}, UserId: {UserId}.",
                    returnRequest.Id,
                    returnRequest.StoreOrderId,
                    returnRequest.UserId);

                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    await _paymentService.RefundReturnRequestAsync(returnRequest);

                    returnRequest.MarkAsRefunded();
                    _returnRequestRepository.Update(returnRequest);

                    await _unitOfWork.CommitTransactionAsync();

                    successCount++;

                    _logger.LogInformation(
                        "Refund completed successfully for ReturnRequestId: {ReturnRequestId}.",
                        returnRequest.Id);
                }
                catch (Exception ex)
                {
                    failedCount++;

                    _logger.LogError(
                        ex,
                        "Refund failed for ReturnRequestId: {ReturnRequestId}, " +
                        "StoreOrderId: {StoreOrderId}, UserId: {UserId}.",
                        returnRequest.Id,
                        returnRequest.StoreOrderId,
                        returnRequest.UserId);

                    try
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogCritical(
                            rollbackEx,
                            "Rollback failed for ReturnRequestId: {ReturnRequestId}.",
                            returnRequest.Id);
                    }
                }
            }

            _logger.LogInformation(
                "Refund processing finished. Total: {TotalCount}, " +
                "Succeeded: {SuccessCount}, Failed: {FailedCount}.",
                totalCount,
                successCount,
                failedCount);

            return SimpleResult.Success();
        }

        public async Task<SimpleResult> RefundReturnRequestAsync(Guid returnRequestId)
        {
            var returnRequest = await _returnRequestRepository.GetByIdAsync(returnRequestId);

            if (returnRequest is null )
            {
                throw new InvalidOperationException($"Return request with ID {returnRequestId} not found.");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _paymentService.RefundReturnRequestAsync(returnRequest);
                returnRequest.MarkAsRefunded();
                _returnRequestRepository.Update(returnRequest);
                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation(
                    "Refund completed successfully for ReturnRequestId: {ReturnRequestId}.",
                    returnRequest.Id);
                return SimpleResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Refund failed for ReturnRequestId: {ReturnRequestId}, " +
                    "StoreOrderId: {StoreOrderId}, UserId: {UserId}.",
                    returnRequest.Id,
                    returnRequest.StoreOrderId,
                    returnRequest.UserId);
                try
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogCritical(
                        rollbackEx,
                        "Rollback failed for ReturnRequestId: {ReturnRequestId}.",
                        returnRequest.Id);
                }
                return SimpleResult.Failure($"Refund failed for ReturnRequestId: {returnRequest.Id}.");
            }
        }
    }
}
