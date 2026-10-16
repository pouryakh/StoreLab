using Application.DTOs.OrderDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.Dapper
{
    public interface IOrderReadRepository
    {
        Task<PaginatedResult<OrderResponse>> GetOrdersAsync(Guid userId, OrdersPaginationRequest request);

        Task<OrderResponse?> GetOrderByIdAsync(Guid userId,Guid orderId);
    }
}
