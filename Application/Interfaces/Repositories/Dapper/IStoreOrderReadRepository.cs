using Application.DTOs.OrderDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.Dapper
{
    public interface IStoreOrderReadRepository
    {
        Task<PaginatedResult<StoreOrderResponse>> GetStoreOrdersAsync(Guid userId, OrdersPaginationRequest request);

        Task<StoreOrderResponse?> GetStoreOrderByIdAsync(Guid userId, Guid storeOrderId);
    }
}
