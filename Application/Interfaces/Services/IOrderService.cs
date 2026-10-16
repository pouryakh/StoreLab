using Application.DTOs.CommonDto;
using Application.DTOs.OrderDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Result<PaginatedResult<OrderResponse>>> GetOrders(OrdersPaginationRequest request);

        Task<Result<OrderResponse>> GetOrderById(Guid orderId);
        
    }
}
