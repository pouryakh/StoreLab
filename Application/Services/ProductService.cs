using Application.DTOs.ProductDto;
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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ProductService> _logger;

        private readonly ICurrentUserService _currentUserService;

        private readonly IProductRepository _productRepository;

        private readonly IValidator<CreateProductRequestDto> _productRequestValidator;
        private readonly IValidator<ChangeStockRequest> _changeStockValidator;

        private readonly IValidator<UpdateProductRequest> _updateProductValidator;

        private readonly IStoreRepository _storeRepository;

        public ProductService(
            IUnitOfWork unitOfWork,
            ILogger<ProductService> logger,
            ICurrentUserService currentUserService,
            IValidator<CreateProductRequestDto> productRequestValidator,
            IValidator<ChangeStockRequest> changeStockValidator,
            IValidator<UpdateProductRequest> updateProductValidator,
            IStoreRepository storeRepository,
            IProductRepository productRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
            _productRepository = productRepository;
            _productRequestValidator = productRequestValidator;
            _changeStockValidator = changeStockValidator;
            _updateProductValidator = updateProductValidator;
            _storeRepository = storeRepository;
        }

        public async Task<SimpleResult> ActivateProductAsync(Guid productId)
        {
            _logger.LogInformation(
                "Activating Product {ProductId}. UserId: {UserId}",
                productId,
                _currentUserService.UserId);

            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
            {
                _logger.LogWarning(
                    "Product {ProductId} was not found.",
                    productId);

                return SimpleResult.Failure("Product not Found");
            }

            if (product.IsActive)
            {
                _logger.LogWarning(
                    "Product {ProductId} is already active.",
                    productId);

                return SimpleResult.Failure("Product is already active.");
            }

            if (product.Store.SellerId != _currentUserService.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to activate Product {ProductId} without permission.",
                    _currentUserService.UserId,
                    productId);

                return SimpleResult.Failure("Access Dinied");
            }

            if (product.Store.Approval == Domain.Enums.StoreApprovalStatus.Rejected)
            {
                _logger.LogWarning(
                    "Store {StoreId} Was Rejected and Try tp active an product",
                    product.StoreId);

                return SimpleResult.Failure("Store Is Rejected");
            }

            product.Activate();

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Product {ProductId} activated successfully by User {UserId}.",
                productId,
                _currentUserService.UserId);

            return SimpleResult.Success();
        }

        public async Task<SimpleResult> ChangeStockAsync(ChangeStockRequest request)
        {
            _logger.LogInformation(
                "Changing stock for Product {ProductId}. UserId: {UserId}, RequestedStock: {NewStock}",
                request.ProductId,
                _currentUserService.UserId,
                request.NewStock);

            var validationResult = await _changeStockValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Invalid change stock request for Product {ProductId}. UserId: {UserId}",
                    request.ProductId,
                    _currentUserService.UserId);

                return SimpleResult.Failure(validationResult.Errors.First().ErrorMessage);

            }

            var product = await _productRepository.GetByIdAsync(request.ProductId);

            if (product is null)
            {
                _logger.LogWarning(
                    "Product {ProductId} was not found.",
                    request.ProductId);

                return SimpleResult.Failure("Product not found.");
            }

            if (product.Store.SellerId != _currentUserService.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to change stock for Product {ProductId} without permission.",
                    _currentUserService.UserId,
                    request.ProductId);

                return SimpleResult.Failure("Access denied.");
            }

            var oldStock = product.StockQuantity;

            if (oldStock < request.NewStock)
            {
                product.IncreaseStock(request.NewStock - oldStock);
            }
            else if (oldStock > request.NewStock)
            {
                product.DecreaseStock(oldStock - request.NewStock);
            }
            else
            {
                _logger.LogInformation(
                    "Stock for Product {ProductId} is already {StockQuantity}. No changes made.",
                    request.ProductId,
                    oldStock);

                return SimpleResult.Failure("Stock quantity is already set to this value.");
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Stock for Product {ProductId} changed successfully from {OldStock} to {NewStock} by User {UserId}.",
                request.ProductId,
                oldStock,
                request.NewStock,
                _currentUserService.UserId);

            return SimpleResult.Success();
        }

        public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequestDto request)
        {
            _logger.LogInformation(
                "Creating product for Store {StoreId} by User {UserId}.",
                request.StoreId,
                _currentUserService.UserId);

            if (!_currentUserService.IsAuthenticated ||
                !_currentUserService.UserId.HasValue)
            {
                _logger.LogWarning(
                    "Unauthenticated user attempted to create a product for Store {StoreId}.",
                    request.StoreId);

                return Result<ProductResponse>.Failure(
                    "Please login first.");
            }

            var validationResult =
                await _productRequestValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Invalid product creation request for Store {StoreId} by User {UserId}.",
                    request.StoreId,
                    _currentUserService.UserId);

                return Result<ProductResponse>.Failure(validationResult.Errors.First().ErrorMessage);
                    
            }

            var isOwner = await _storeRepository.IsOwnerOrNot(
                _currentUserService.UserId.Value,
                request.StoreId);

            if (!isOwner)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to create a product for Store {StoreId} without permission.",
                    _currentUserService.UserId,
                    request.StoreId);

                return Result<ProductResponse>.Failure(
                    "Access denied.");
            }

            var product = new Product(
                request.StoreId,
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity);

            await _productRepository.AddAsync(product);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Product {ProductId} created successfully for Store {StoreId} by User {UserId}.",
                product.Id,
                request.StoreId,
                _currentUserService.UserId);

            var productDto = new ProductResponse
            {
                Id = product.Id,
                StoreId = product.StoreId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive
            };

            return Result<ProductResponse>.Success(productDto);
        }

        public async Task<SimpleResult> DeactivateProductAsync(Guid productId)
        {
            _logger.LogInformation(
                "Deactivating Product {ProductId}. UserId: {UserId}",
                productId,
                _currentUserService.UserId);

            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
            {
                _logger.LogWarning(
                    "Product {ProductId} was not found.",
                    productId);

                return SimpleResult.Failure("Product not Found");
            }

            if (!product.IsActive)
            {
                _logger.LogWarning(
                    "Product {ProductId} is already deactive.",
                    productId);

                return SimpleResult.Failure("Product is already deactive.");
            }

            if (product.Store.SellerId != _currentUserService.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to deactivate Product {ProductId} without permission.",
                    _currentUserService.UserId,
                    productId);

                return SimpleResult.Failure("Access Dinied");
            }

            product.Deactivate();

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Product {ProductId} deactivated successfully by User {UserId}.",
                productId,
                _currentUserService.UserId);

            return SimpleResult.Success();
        }

        public async Task<Result<ProductResponse>> UpdateProductAsync(Guid productId,UpdateProductRequest request)
        {
            _logger.LogInformation(
                "Updating Product {ProductId} by User {UserId}.",
                productId,
                _currentUserService.UserId);

            var validationResult =
                await _updateProductValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Invalid update request for Product {ProductId}. UserId: {UserId}",
                    productId,
                    _currentUserService.UserId);

                return Result<ProductResponse>.Failure(
                    string.Join(" ",
                        validationResult.Errors.Select(x => x.ErrorMessage)));
            }

            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
            {
                _logger.LogWarning(
                    "Product {ProductId} was not found.",
                    productId);

                return Result<ProductResponse>.Failure(
                    "Product not found.");
            }

            if (product.Store.SellerId != _currentUserService.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to update Product {ProductId} without permission.",
                    _currentUserService.UserId,
                    productId);

                return Result<ProductResponse>.Failure(
                    "Access denied.");
            }

            product.UpdateDetails(
                request.Name,
                request.Description,
                request.Price);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Product {ProductId} updated successfully by User {UserId}.",
                productId,
                _currentUserService.UserId);

            var productDto = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                StoreId = product.StoreId,
                StoreName = product.Store.Name
            };

            return Result<ProductResponse>.Success(productDto);
        }
    }
}
