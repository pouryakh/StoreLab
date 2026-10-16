using Application.DTOs.ProductDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Dapper;
using Application.Interfaces.Services;
using Application.Results;
using Application.Validators.Product;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductCatalogService : IProductCatalogService
    {
        private readonly ILogger<ProductCatalogService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IProductRepository _productRepository;

        private readonly IProductReadRepository _productReadRepository;

        private readonly IValidator<ProductQueryRequest> _productQueryValidator;

        public ProductCatalogService(
            ILogger<ProductCatalogService> logger,
            IUnitOfWork unitOfWork,
            IProductRepository productRepository,
            IProductReadRepository productRepositoryDapper,
            IValidator<ProductQueryRequest> validator1
            )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _productReadRepository = productRepositoryDapper;
            _productQueryValidator = validator1;
        }

        public async Task<Result<ProductResponse>> GetProductById(Guid productId)
        {
            _logger.LogInformation(
                "Getting product {ProductId} for catalog.",
                productId);

            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null || !product.IsActive)
            {
                _logger.LogWarning(
                    "Product {ProductId} was not found or is inactive.",
                    productId);

                return Result<ProductResponse>.Failure("Product Not Found");
            }

            if (product.StockQuantity <= 0)
            {
                _logger.LogWarning(
                    "Product {ProductId} is out of stock.",
                    productId);

                return Result<ProductResponse>.Failure("Product Is Out Of Stock");
            }

            var productDto = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                IsActive = product.IsActive,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                StoreId = product.StoreId,
                StoreName = product.Store.Name
            };

            _logger.LogInformation(
                "Product {ProductId} retrieved successfully.",
                productId);

            return Result<ProductResponse>.Success(productDto);
        }

        public async Task<Result<PaginatedResult<ProductResponse>>> GetProductsAsync(ProductQueryRequest request)
        {
            _logger.LogInformation(
                "Getting products. Page: {Page}, PageSize: {PageSize}, Search: {Search}, StoreId: {StoreId}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}",
                request.Page,
                request.PageSize,
                request.Search,
                request.StoreId,
                request.MinPrice,
                request.MaxPrice);

            var validationResult = await _productQueryValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Faild cuse - {validationEror}", validationResult.Errors.First().ErrorMessage);
                return Result<PaginatedResult<ProductResponse>>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            var result = await _productReadRepository.GetProductsAsync(request);

            _logger.LogInformation(
                "Products retrieved successfully. Returned: {ItemCount}, TotalCount: {TotalCount}, Page: {Page}",
                result.Items.Count,
                result.TotalCount,
                result.PageNumber);

            return Result<PaginatedResult<ProductResponse>>.Success(result);
        }


    }
}
