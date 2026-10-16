using Application.Common.Extensions;
using Application.DTOs.OrderDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Dapper;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Common.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class StoreOrderService : IStoreOrderService
    {
        private readonly IStoreOrderRepository _storeOrderRepository;

        private readonly IStoreOrderReadRepository _storeOrderReadRepository;

        private readonly ICurrentUserService _currentUserService;

        private readonly ILogger<StoreOrderService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public StoreOrderService(
            IStoreOrderRepository storeOrderRepository,
            ICurrentUserService currentUserService,
            ILogger<StoreOrderService> logger,
            IUnitOfWork unitOfWork,
            IStoreOrderReadRepository readRepository
            )
        {
            _storeOrderRepository = storeOrderRepository;
            _currentUserService = currentUserService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _storeOrderReadRepository = readRepository;
        }

        public async Task<Result<StoreOrderResponse>> GetStoreOrderById(Guid storeOrderId)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting store order {StoreOrderId} for User {UserId}.",
                storeOrderId,
                userId);

            var response =
                await _storeOrderReadRepository.GetStoreOrderByIdAsync(
                    userId,
                    storeOrderId);

            if (response is null)
            {
                _logger.LogWarning(
                    "Store order {StoreOrderId} was not found for User {UserId}.",
                    storeOrderId,
                    userId);

                return Result<StoreOrderResponse>.Failure(
                    "Store order was not found.");
            }

            _logger.LogInformation(
                "Store order {StoreOrderId} retrieved successfully for User {UserId}.",
                storeOrderId,
                userId);

            return Result<StoreOrderResponse>.Success(response);
        }

        public async Task<Result<PaginatedResult<StoreOrderResponse>>> GetStoreOrders(OrdersPaginationRequest request)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting store orders for User {UserId}. Page {PageNumber}, PageSize {PageSize}.",
                userId,
                request.PageNumber,
                request.PageSize);

            var response =
                await _storeOrderReadRepository.GetStoreOrdersAsync(
                    userId,
                    request);

            _logger.LogInformation(
                "Store orders retrieved successfully for User {UserId}. Page {PageNumber}, PageSize {PageSize}, TotalCount {TotalCount}.",
                userId,
                request.PageNumber,
                request.PageSize,
                response.TotalCount);

            return Result<PaginatedResult<StoreOrderResponse>>.Success(
                response);
        }

        public async Task<SimpleResult> MarkAsProcessingAsync(Guid storeOrderId) 
        {
            _logger.LogInformation("MarkAsProcessing started for StoreOrder {StoreOrderId}.", storeOrderId);

            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue) 
            { 
                _logger.LogWarning("Unauthorized attempt to process StoreOrder {StoreOrderId}.", storeOrderId);
                throw new UnauthorizedAccessException("User is not authenticated."); 
            }

            var userId = _currentUserService.UserId.Value;

            var storeOrder = await _storeOrderRepository.GetByIdAsync(storeOrderId); 
            
            if (storeOrder is null) 
            {
                _logger.LogWarning("StoreOrder {StoreOrderId} was not found.", storeOrderId);
                throw new DomainException("StoreOrder not found.");
            }

            var isOwner = await _storeOrderRepository.IsStoreOrderOwnedBySeller(storeOrderId, userId);
            if (!isOwner) 
            {
                _logger.LogWarning("User {UserId} attempted to process StoreOrder {StoreOrderId}, " + "but is not the owner of the Store.", userId, storeOrderId);
                throw new UnauthorizedAccessException("You are not allowed to process this StoreOrder.");
            }

            storeOrder.StartProcessing();

            await _unitOfWork.SaveChangesAsync(); 

            _logger.LogInformation("StoreOrder {StoreOrderId} marked as Processing by User {UserId}.", storeOrderId, userId); 
            
            return SimpleResult.Success();
        }

        public async Task<SimpleResult> MarkAsShippedAsync(Guid storeOrderId)
        {
            _logger.LogInformation(
                "MarkAsShipped started for StoreOrder {StoreOrderId}.",
                storeOrderId);

            if (!_currentUserService.IsAuthenticated ||
                !_currentUserService.UserId.HasValue)
            {
                _logger.LogWarning(
                    "Unauthorized attempt to ship StoreOrder {StoreOrderId}.",
                    storeOrderId);

                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var userId = _currentUserService.UserId.Value;

            var storeOrder =
                await _storeOrderRepository.GetByIdAsync(storeOrderId);

            if (storeOrder is null)
            {
                _logger.LogWarning(
                    "StoreOrder {StoreOrderId} was not found while attempting to ship it.",
                    storeOrderId);

                throw new DomainException(
                    "StoreOrder not found.");
            }

            var isOwner =
                await _storeOrderRepository
                    .IsStoreOrderOwnedBySeller(storeOrderId, userId);

            if (!isOwner)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to ship StoreOrder {StoreOrderId}, " +
                    "but is not the owner of the Store.",
                    userId,
                    storeOrderId);

                throw new UnauthorizedAccessException(
                    "You are not allowed to ship this StoreOrder.");
            }

            storeOrder.MarkAsShipped();

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "StoreOrder {StoreOrderId} marked as Shipped by User {UserId}.",
                storeOrderId,
                userId);

            return SimpleResult.Success();
        }
    }
}
