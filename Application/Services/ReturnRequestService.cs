using Application.Common.Extensions;
using Application.DTOs.ReturnRequestDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Dapper;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Common.Exceptions;
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
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ReturnRequestService> _logger;

        private readonly IReturnRequestRepository _returnRequestRepository;

        private readonly IOrderRepository _orderRepository;

        private readonly IStoreOrderRepository _storeOrderRepository;

        private readonly IRefundService _refundService;

        private readonly IReturnRequestReadRepository _returnRequestReadRepository;

        private readonly ICurrentUserService _currentUserService;

        private readonly IPaymentService _paymentService;

        private readonly IProductRepository _productRepository;

        private readonly IStoreRepository _storeRepository;

        public ReturnRequestService(IUnitOfWork unitOfWork,
            ILogger<ReturnRequestService> logger,
            IReturnRequestRepository returnRequestRepository,
            IOrderRepository orderRepository,
            ICurrentUserService currentUserService,
            IStoreOrderRepository storeOrderRepository,
            IStoreRepository storeRepository,
            IReturnRequestReadRepository returnRequestReadRepository,
            IRefundService refundService,
            IPaymentService paymentService,
            IProductRepository productRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _returnRequestRepository = returnRequestRepository;
            _orderRepository = orderRepository;
            _currentUserService = currentUserService;
            _storeOrderRepository = storeOrderRepository;
            _storeRepository = storeRepository;
            _returnRequestReadRepository = returnRequestReadRepository;
            _refundService = refundService;
            _paymentService = paymentService;
            _productRepository = productRepository;
        }

        public async Task<SimpleResult> CancelStoreOrderAsync(Guid storeOrderId)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "User {UserId} initiated cancellation for StoreOrder {StoreOrderId}.",
                userId,
                storeOrderId);

            if (!await _storeOrderRepository.BelongsToUserAsync(
                    storeOrderId, userId))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to cancel StoreOrder {StoreOrderId}, but the order was not found or does not belong to the user.",
                    userId,
                    storeOrderId);

                return SimpleResult.Failure(
                    "Store order was not found or does not belong to the current user.");
            }

            var storeOrder =
                await _storeOrderRepository.GetByIdAsync(storeOrderId);

            if (storeOrder is null)
            {
                _logger.LogWarning(
                    "StoreOrder {StoreOrderId} was not found while processing cancellation for User {UserId}.",
                    storeOrderId,
                    userId);

                return SimpleResult.Failure(
                    "Store order was not found.");
            }

            if (storeOrder.SettlementStatus ==
                Domain.Enums.SettlementStatus.Eligible)
            {
                _logger.LogWarning(
                    "Cancellation rejected for StoreOrder {StoreOrderId}, User {UserId}. Settlement status is {SettlementStatus}.",
                    storeOrderId,
                    userId,
                    storeOrder.SettlementStatus);

                return SimpleResult.Failure(
                    "The store order cannot be cancelled because its settlement is no longer pending.");
            }

            if (storeOrder.Status ==
                Domain.Enums.StoreOrderStatus.Cancelled)
            {
                _logger.LogWarning(
                    "Cancellation rejected for StoreOrder {StoreOrderId}, User {UserId}. The order is already cancelled.",
                    storeOrderId,
                    userId);

                return SimpleResult.Failure(
                    "Store order is already cancelled.");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                storeOrder.BlockSettlement();

                storeOrder.Cancel();

                storeOrder.Order.ChangeRefundAmount(storeOrder.TotalAmount);

                await RefactorProductStock(storeOrder);

                _logger.LogInformation(
                    "StoreOrder {StoreOrderId} cancellation validated. Processing refund of {RefundAmount} for User {UserId}.",
                    storeOrderId,
                    storeOrder.TotalAmount,
                    userId);

                var refundResult =
                    await _paymentService.RefundCancelOrderAsync(storeOrder, userId);

                if (!refundResult.IsSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    _logger.LogWarning(
                        "Refund failed for StoreOrder {StoreOrderId}, User {UserId}. Cancellation transaction rolled back.",
                        storeOrderId,
                        userId);

                    return SimpleResult.Failure(
                        "Store order cancellation failed because the refund could not be completed.");
                }

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "StoreOrder {StoreOrderId} cancelled and refunded successfully. UserId: {UserId}, RefundAmount: {RefundAmount}.",
                    storeOrderId,
                    userId,
                    storeOrder.TotalAmount);

                return SimpleResult.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "An error occurred while cancelling StoreOrder {StoreOrderId} for User {UserId}. RefundAmount: {RefundAmount}.",
                    storeOrderId,
                    userId,
                    storeOrder.TotalAmount);

                throw;
            }
        }

        public async Task<Result<ReturnRequestRsponse>> CreateReturnRequest(CreateReturnRequestDto request)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "CreateReturnRequest started for StoreOrder {StoreOrderId} by User {UserId}.",
                request.StoreOrderId,
                userId);

            if (!await _storeOrderRepository.BelongsToUserAsync(
                    request.StoreOrderId, userId))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to create a return request for StoreOrder {StoreOrderId}, but the order was not found or does not belong to the user.",
                    userId,
                    request.StoreOrderId);

                return Result<ReturnRequestRsponse>.Failure(
                    "Store order was not found or does not belong to the current user.");
            }

            var storeOrder =
                await _storeOrderRepository.GetByIdAsync(request.StoreOrderId);

            if (storeOrder is null)
            {
                _logger.LogWarning(
                    "StoreOrder {StoreOrderId} was not found while creating a return request for User {UserId}.",
                    request.StoreOrderId,
                    userId);

                return Result<ReturnRequestRsponse>.Failure(
                    "Store order was not found.");
            }

            if (await _returnRequestRepository
                    .IsAnyReturnRequestWithThisStoreOrderExsist(request.StoreOrderId))
            {
                _logger.LogWarning(
                    "A return request already exists for StoreOrder {StoreOrderId}. User {UserId} cannot create another request.",
                    request.StoreOrderId,
                    userId);

                return Result<ReturnRequestRsponse>.Failure(
                    "A return request already exists for this store order.");
            }

            if (storeOrder.SettlementStatus !=
                Domain.Enums.SettlementStatus.Pending)
            {
                _logger.LogWarning(
                    "Return request rejected for StoreOrder {StoreOrderId}. Settlement status is {SettlementStatus}.",
                    request.StoreOrderId,
                    storeOrder.SettlementStatus);

                return Result<ReturnRequestRsponse>.Failure(
                    "A return request cannot be created because the store order settlement is no longer pending.");
            }

            if (storeOrder.Status !=
                Domain.Enums.StoreOrderStatus.Delivered)
            {
                _logger.LogWarning(
                    "Return request rejected for StoreOrder {StoreOrderId}. Current status is {StoreOrderStatus}.",
                    request.StoreOrderId,
                    storeOrder.Status);

                return Result<ReturnRequestRsponse>.Failure(
                    "A return request can only be created for a delivered store order.");
            }

            var returnRequest = new ReturnRequest(
                request.StoreOrderId,
                userId,
                request.Reason);

            returnRequest.Approve();

            storeOrder.BlockSettlement();

            storeOrder.Order.ChangeRefundAmount(storeOrder.TotalAmount);

            await _returnRequestRepository.AddAsync(returnRequest);
            await _unitOfWork.SaveChangesAsync();

            var response = new ReturnRequestRsponse
            {
                Id = returnRequest.Id,
                Reason = returnRequest.Reason,
                RequestedAt = returnRequest.RequestedAt,
                Status = returnRequest.Status,
                StoreOrderId = returnRequest.StoreOrderId
            };

            _logger.LogInformation(
                "Return request {ReturnRequestId} created successfully for StoreOrder {StoreOrderId} by User {UserId}.",
                returnRequest.Id,
                request.StoreOrderId,
                userId);

            return Result<ReturnRequestRsponse>.Success(response);
        }

        public async Task<Result<PaginatedResult<ReturnRequestRsponse>>> GetMyReturnRequest(ReturnRequestPaginationRequest request)
        {
            var userId = CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting return requests for User {UserId}.",
                userId);

            var response = await _returnRequestReadRepository
                .GetAllUserReturnRequestsAsync(request, userId);

            _logger.LogInformation(
                "Retrieved {Count} return requests for User {UserId}.",
                response.Items.Count,
                userId);

            return Result<PaginatedResult<ReturnRequestRsponse>>.Success(response);
        }

        public async Task<Result<ReturnRequestRsponse>> GetReturnRequest(Guid returnRequestId)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "GetReturnRequest started for ReturnRequest {ReturnRequestId} by User {UserId}.",
                returnRequestId,
                userId);

            var returnRequest =
                await _returnRequestRepository.GetByIdAsync(returnRequestId);

            if (returnRequest is null)
            {
                _logger.LogWarning(
                    "ReturnRequest {ReturnRequestId} was not found for User {UserId}.",
                    returnRequestId,
                    userId);

                return Result<ReturnRequestRsponse>.Failure(
                    "Return request was not found.");
            }

            if (returnRequest.UserId != userId &&
                _currentUserService.Role != UserRole.Admin.ToString() &&
                !await _storeRepository.IsOwnerOfStoreOrder(returnRequestId, userId))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access ReturnRequest {ReturnRequestId} without permission.",
                    userId,
                    returnRequestId);

                return Result<ReturnRequestRsponse>.Failure(
                    "You are not authorized to access this return request.");
            }

            var response = new ReturnRequestRsponse
            {
                Id = returnRequest.Id,
                Reason = returnRequest.Reason,
                ReceivedAt = returnRequest.ReceivedAt,
                RequestedAt = returnRequest.RequestedAt,
                ResolvedAt = returnRequest.ResolvedAt,
                Status = returnRequest.Status,
                StoreOrderId = returnRequest.StoreOrderId
            };

            _logger.LogInformation(
                "ReturnRequest {ReturnRequestId} retrieved successfully by User {UserId}.",
                returnRequestId,
                userId);

            return Result<ReturnRequestRsponse>.Success(response);
        }

        public async Task<Result<PaginatedResult<ReturnRequestRsponse>>> GetReturnRequestForAdmins(ReturnRequestPaginationRequest request)
        {
            var userId = CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Admin {UserId} is retrieving all return requests.",
                userId);

            var response = await _returnRequestReadRepository
                .GetAllReturnRequestAsync(request);

            _logger.LogInformation(
                "Admin {UserId} retrieved all return requests successfully.",
                userId);

            return Result<PaginatedResult<ReturnRequestRsponse>>.Success(response);
        }

        public async Task<Result<PaginatedResult<ReturnRequestRsponse>>> GetSellerReturnRequest(ReturnRequestPaginationRequest request)
        {
            var userId = CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Seller {UserId} is retrieving return requests for their store.",
                userId);

            var response = await _returnRequestReadRepository
                .GetAllStoreReturnRequestsAsync(request, userId);

            _logger.LogInformation(
                "Seller {UserId} retrieved return requests successfully. Page: {PageNumber}, PageSize: {PageSize}.",
                userId,
                request.PageNumber,
                request.PageSize);

            return Result<PaginatedResult<ReturnRequestRsponse>>.Success(response);
        }

        public async Task<SimpleResult> MarkAsReturned(Guid returnRequestId)
        {
            var userId = CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "MarkAsReturned started for ReturnRequest {ReturnRequestId} by User {UserId}.",
                returnRequestId,
                userId);

            var returnRequest = await _returnRequestRepository.GetByIdAsync(returnRequestId);

            if (returnRequest is null)
            {
                _logger.LogWarning(
                    "ReturnRequest {ReturnRequestId} was not found. User {UserId} attempted to mark it as received.",
                    returnRequestId,
                    userId);

                return SimpleResult.Failure("Return request was not found.");
            }

            if (!await _storeRepository.IsOwnerOfStoreOrder(
                    returnRequest.StoreOrderId, userId))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to mark ReturnRequest {ReturnRequestId} as received without owning StoreOrder {StoreOrderId}.",
                    userId,
                    returnRequestId,
                    returnRequest.StoreOrderId);

                return SimpleResult.Failure(
                    "You are not authorized to mark this return as received.");
            }

            var storeOrder = await _storeOrderRepository.GetByIdAsync(returnRequest.StoreOrderId);

            if (storeOrder is null)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to mark ReturnRequest {ReturnRequestId} as received But We Cant Find  StoreOrder {StoreOrderId}.",
                    userId,
                    returnRequestId,
                    returnRequest.StoreOrderId);

                return SimpleResult.Failure(
                    "Store Order Not Found");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                returnRequest.MarkAsReceived();

                await _refundService.RefundReturnRequestAsync(returnRequestId);

                await RefactorProductStock(storeOrder);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "ReturnRequest {ReturnRequestId} marked as received and refunded successfully.",
                    returnRequestId);

                return SimpleResult.Success();
            }
            catch (DomainException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogWarning(
                    ex,
                    "ReturnRequest {ReturnRequestId} could not be marked as received due to a domain rule.",
                    returnRequestId);

                return SimpleResult.Failure(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogWarning(
                    ex,
                    "ReturnRequest {ReturnRequestId} refund could not be completed due to an invalid operation.",
                    returnRequestId);

                return SimpleResult.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Unexpected error while marking ReturnRequest {ReturnRequestId} as received and processing its refund.",
                    returnRequestId);

                throw;
            }

        }

        private async Task RefactorProductStock(StoreOrder storeOrder)
        {
            _logger.LogInformation(
                "Starting product stock restoration for StoreOrder {StoreOrderId}.",
                storeOrder.Id);

            foreach (OrderItem item in storeOrder.Items)
            {
                _logger.LogInformation(
                    "Restoring stock for Product {ProductId}. Quantity: {Quantity}.",
                    item.ProductId,
                    item.Quantity);

                var product = await _productRepository.GetByIdAsync(item.ProductId);

                if (product is null)
                {
                    _logger.LogError(
                        "Product {ProductId} not found while restoring stock for StoreOrder {StoreOrderId}.",
                        item.ProductId,
                        storeOrder.Id);

                    throw new InvalidOperationException(
                        $"Product with Id {item.ProductId} not found.");
                }

                product.IncreaseStock(item.Quantity);

                _logger.LogInformation(
                    "Stock restored successfully for Product {ProductId}. Quantity: {Quantity}.",
                    item.ProductId,
                    item.Quantity);
            }

            _logger.LogInformation(
                "Product stock restoration completed for StoreOrder {StoreOrderId}.",
                storeOrder.Id);
        }
    }
}
