using Application.DTOs.ShoppingCartDto;
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

namespace Application.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICurrentUserService _currentUserService;

        private readonly IUserRepository _userRepository;

        private readonly IProductRepository _productRepository;

        private readonly IValidator<AddItemRequest> _addValidator;

        private readonly IValidator<RemoveItemRequest> _removeValidator;

        private readonly IValidator<UpdateItemQuantityRequest> _updateQuantityValidator;

        private readonly ILogger<ShoppingCartService> _logger;

        private readonly ICartItemRepository _cartItemRepository;

        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository,
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IValidator<AddItemRequest> addValidator,
            IValidator<RemoveItemRequest> removeValidator,
            IValidator<UpdateItemQuantityRequest> updateQuantityValidator,
            ILogger<ShoppingCartService> logger,
            ICartItemRepository cartItemRepository,
            IUnitOfWork unitOfWork)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _addValidator = addValidator;
            _removeValidator = removeValidator;
            _updateQuantityValidator = updateQuantityValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<SimpleResult> AddItem(AddItemRequest request)
        {
            _logger.LogInformation(
                "User: {UserId} Start To Add Item To Shopping Cart",
                _currentUserService.UserId);

            var cart = await GetCurrentUserCartAsync();

            var product = await _productRepository.GetByIdAsync(request.ProductId);

            if (product is null)
            {
                _logger.LogWarning(
                    "Invalid Product ProductId: {ProductId}",
                    request.ProductId);

                return SimpleResult.Failure("Invalid Product");
            }

            cart.AddItem(product, request.Quantity);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Product: {ProductId} was added to CartId: {CartId} for User: {UserId}",
                request.ProductId,
                cart.Id,
                cart.UserId);

            return SimpleResult.Success();
        }

        public async Task<SimpleResult> ClearShoppingCart()
        {
            _logger.LogInformation(
                "User: {UserId} Start To Clear Shopping Cart",
                _currentUserService.UserId);

            var cart = await GetCurrentUserCartAsync();

            cart.ClearCart();

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Shopping cart: {CartId} cleared",
                cart.Id);

            return SimpleResult.Success();
        }

        public async Task<Result<ShoppingCartResponse>> GetShoppingCart()
        {
            _logger.LogInformation(
                "User: {UserId} Start To Get Shopping Cart",
                _currentUserService.UserId);

            var cart = await GetCurrentUserCartAsync();

            var cartResponse = new ShoppingCartResponse()
            {
                Items = cart.Items.Select(item => new CartItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList(),
                totalAmount = cart.CalculateTotalAmount()
            };

            _logger.LogInformation($"Shopping cart: {cart.Id} Send SeccussFully");

            return Result<ShoppingCartResponse>.Success( cartResponse );
        }

        public async Task<SimpleResult> RemoveItem(RemoveItemRequest request)
        {
            _logger.LogInformation(
                "User: {UserId} Start To Remove Item From Shopping Cart",
                _currentUserService.UserId);

            var cart = await GetCurrentUserCartAsync();

            cart.RemoveItem(request.CartItemId);

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation(
                "Remove Item: {CartItemId} From Cart: {CartId}",
                request.CartItemId,
                cart.Id);

            return SimpleResult.Success();
        }

        public async Task<SimpleResult> UpdateItemQuantity(UpdateItemQuantityRequest request)
        {
            _logger.LogInformation(
                "User: {UserId} Start To UpdateItemQuantity In Cart",
                _currentUserService.UserId);

            var cart = await GetCurrentUserCartAsync();

            var product = await _productRepository.GetByIdAsync(request.ProductId);

            if (product is null)
            {
                _logger.LogWarning(
                    "Invalid Product ProductId: {ProductId}",
                    request.ProductId);

                return SimpleResult.Failure("Invalid Product");
            }

            cart.ChangeQuantity(request.NewQuantity,product);

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("UpdateItemQuantity For Cart {cartId} Done.",cart.Id);

            return SimpleResult.Success();

        }

        private async Task<ShoppingCart> GetCurrentUserCartAsync()
        {
            if (!_currentUserService.IsAuthenticated ||
                _currentUserService.UserId == null)
            {
                throw new UnauthorizedAccessException(
                    "Please Login Or Register First");
            }

            var cart = await _shoppingCartRepository
                .GetCartByUserId(_currentUserService.UserId.Value);

            if (cart == null)
            {
                _logger.LogError(
                    "ShoppingCart not found for UserId: {UserId}",
                    _currentUserService.UserId);

                throw new InvalidOperationException(
                    "ShoppingCart was not found for current user.");
            }

            return cart;
        }
    }
}
