using Application.Common.Extensions;
using Application.Common.Helpers;
using Application.DTOs.OrderDto;
using Application.Interfaces.Repositories.Dapper;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Results;
using Application.Validators.Order;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderService : IOrderService
    {

        private readonly ILogger<OrderService> _logger;

        private readonly IOrderReadRepository _orderReadRepository;

        private readonly ICurrentUserService _currentUserService;

        private readonly IValidator<OrdersPaginationRequest> _validator;

        public OrderService(
            ILogger<OrderService> logger,
            IOrderReadRepository orderReadRepository,
            ICurrentUserService currentUserService,
            IValidator<OrdersPaginationRequest> validator
            )
        {
            _logger = logger;
            _orderReadRepository = orderReadRepository;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task<Result<OrderResponse>> GetOrderById(Guid orderId)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting order {OrderId} for User {UserId}.",
                orderId,
                userId);

            var orderResponse =
                await _orderReadRepository.GetOrderByIdAsync(
                    userId,
                    orderId);

            if (orderResponse is null)
            {
                _logger.LogWarning(
                    "Order {OrderId} was not found for User {UserId}.",
                    orderId,
                    userId);

                return Result<OrderResponse>.Failure(
                    "Order was not found.");
            }

            _logger.LogInformation(
                "Order {OrderId} retrieved successfully for User {UserId}.",
                orderId,
                userId);

            return Result<OrderResponse>.Success(orderResponse);
        }

        public async Task<Result<PaginatedResult<OrderResponse>>> GetOrders(OrdersPaginationRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation(
                    "Order pagination validation failed for User request. Errors: {Error}",
                    validationResult.Errors.First().ErrorMessage);

                return Result<PaginatedResult<OrderResponse>>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting orders for User {UserId}. Page {PageNumber}, PageSize {PageSize}.",
                userId,
                request.PageNumber,
                request.PageSize);

            var ordersResponse =
                await _orderReadRepository.GetOrdersAsync(
                    userId,
                    request);

            _logger.LogInformation(
                "Orders retrieved successfully for User {UserId}. Page {PageNumber}, PageSize {PageSize}, TotalCount {TotalCount}.",
                userId,
                request.PageNumber,
                request.PageSize,
                ordersResponse.TotalCount);

            return Result<PaginatedResult<OrderResponse>>.Success(
                ordersResponse);
        }


    }
}
