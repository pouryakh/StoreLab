using Application.DTOs.ProductDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.Dapper
{
    public interface IProductReadRepository
    {
        Task<PaginatedResult<ProductResponse>> GetProductsAsync(
        ProductQueryRequest request);
    }
}
