using Application.DTOs.ProductDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequestDto request);

        Task<Result<ProductResponse>> UpdateProductAsync(Guid productId, UpdateProductRequest request);

        Task<SimpleResult> ActivateProductAsync(Guid productId);

        Task<SimpleResult> DeactivateProductAsync(Guid productId);

        Task<SimpleResult> ChangeStockAsync(ChangeStockRequest request);
    } 
}
