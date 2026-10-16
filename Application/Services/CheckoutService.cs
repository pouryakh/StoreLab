using Application.Common.Exceptions;
using Application.DTOs.CheakOutDto;
using Application.Interfaces.Repositories;
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
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ICurrentUserService _currentUserService;

        private readonly IShoppingCartRepository _shoppingCartRepository;

        private readonly IOrderRepository _orderRepository;

        private readonly IStoreOrderRepository _storeOrderRepository;

        private readonly ILogger<CheckoutService> _logger;

        private readonly IPaymentService _paymentService;

        private readonly IProductRepository _productRepository;

        public CheckoutService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IShoppingCartRepository shoppingCartRepository,
            IOrderRepository orderRepository,
            IStoreOrderRepository storeOrderRepository,
            IPaymentService paymentService,
            IProductRepository productRepository,
            ILogger<CheckoutService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _shoppingCartRepository = shoppingCartRepository;
            _orderRepository = orderRepository;
            _storeOrderRepository = storeOrderRepository;
            _paymentService = paymentService;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<Result<CheckoutResponse>> CheckoutAsync()
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation(
                "Checkout started for user {UserId}",
                userId);

            if (userId is null)
            {
                _logger.LogWarning(
                    "Checkout failed because user is not authenticated");

                return Result<CheckoutResponse>.Failure(
                    "Please login or SignUp First");
            }

            var userCart = await _shoppingCartRepository
                .GetCartByUserId(userId.Value);

            if (userCart is null)
            {
                _logger.LogError(
                    "Checkout failed: cart not found for user {UserId}",
                    userId);

                throw new InvalidOperationException(
                    "User Doesn't Have Cart");
            }

            if (!userCart.Items.Any())
            {
                _logger.LogWarning(
                    "Checkout failed: cart is empty for user {UserId}",
                    userId);

                return Result<CheckoutResponse>.Failure(
                    "Your cart is empty.");
            }

            var totalAmount = userCart.CalculateTotalAmount();

            _logger.LogInformation(
                "Cart validated for user {UserId}. Items: {ItemCount}, TotalAmount: {TotalAmount}",
                userId,
                userCart.Items.Count,
                totalAmount);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = new Order(
                    userId.Value,
                    totalAmount);

                await _orderRepository.AddAsync(order);

                _logger.LogInformation(
                    "Order {OrderId} created for user {UserId}",
                    order.Id,
                    userId);

                var storeGroups = userCart.Items
                    .GroupBy(x => x.Product.StoreId)
                    .ToList();

                _logger.LogInformation(
                    "Creating {StoreOrderCount} store orders for order {OrderId}",
                    storeGroups.Count,
                    order.Id);

                foreach (var group in storeGroups)
                {
                    var storeOrder = new StoreOrder(
                        order.Id,
                        group.Key);

                    await _storeOrderRepository.AddAsync(storeOrder);

                    var items = group.Select(x => new OrderItem(
                        x.ProductId,
                        storeOrder.Id,
                        x.ProductName,
                        x.Quantity,
                        x.UnitPrice
                    )).ToList();

                    storeOrder.SetItems(items);

                    _logger.LogInformation(
                        "StoreOrder {StoreOrderId} created for store {StoreId} with {ItemCount} items",
                        storeOrder.Id,
                        group.Key,
                        items.Count);
                }

                var productIds = userCart.Items
                    .Select(x => x.ProductId)
                    .ToList();

                var products = await _productRepository
                    .GetProductsByIdsAsync(productIds);

                var productsById = products
                    .ToDictionary(x => x.Id);

                foreach (var item in userCart.Items)
                {
                    if (!productsById.TryGetValue(
                        item.ProductId,
                        out var product))
                    {
                        _logger.LogError(
                            "Product {ProductId} from cart was not found during checkout for user {UserId}",
                            item.ProductId,
                            userId);

                        throw new InvalidOperationException(
                            $"Product {item.ProductId} was not found.");
                    }

                    product.DecreaseStock(item.Quantity);

                    _logger.LogDebug(
                        "Stock decreased for product {ProductId}. Quantity: {Quantity}",
                        product.Id,
                        item.Quantity);
                }

                _logger.LogInformation(
                    "Processing payment for order {OrderId}",
                    order.Id);

                var paymentResult = await _paymentService.PayOrderAsync(order);

                if (paymentResult.IsSuccess is false || paymentResult.Data is null)
                {
                    _logger.LogWarning(
                        "Payment failed for order {OrderId}, user {UserId}",
                        order.Id,
                        userId);

                    throw new DomainException("Payment Failed");
                }

                _logger.LogInformation(
                    "Payment completed successfully for order {OrderId}",
                    order.Id);

                

                userCart.ClearCart();

                _shoppingCartRepository.Update(userCart);

                _logger.LogInformation(
                    "Cart cleared for user {UserId} after successful checkout",
                    userId);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Checkout changes saved successfully for order {OrderId}",
                    order.Id);

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "Checkout transaction committed successfully for order {OrderId}, user {UserId}",
                    order.Id,
                    userId);

                var response = new CheckoutResponse
                {
                    OrderId = order.Id,
                    paymentStatus = Domain.Enums.PaymentStatus.Successful,
                    TotalAmount = order.TotalAmount,
                    RemainingBalance = paymentResult.Data.Balance
                };

                _logger.LogInformation(
                    "Checkout completed successfully for order {OrderId}, user {UserId}",
                    order.Id,
                    userId);

                return Result<CheckoutResponse>.Success(response);
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Checkout concurrency conflict for user {UserId}",
                    userId);

                await _unitOfWork.RollbackTransactionAsync();

                throw;
            }
            catch
            {
                _logger.LogError(
                    "Checkout failed for user {UserId}. Rolling back transaction",
                    userId);

                await _unitOfWork.RollbackTransactionAsync();

                throw;
            }
        }
    }
}
