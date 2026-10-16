using Application.DTOs.ProductDto;
using Application.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IProductCatalogService
    {
        Task<Result<PaginatedResult<ProductResponse>>> GetProductsAsync(ProductQueryRequest request);

        Task<Result<ProductResponse>> GetProductById(Guid productId);
    }
}
