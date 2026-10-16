using Application.DTOs.OrderDto;
using Application.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IStoreOrderService
    {
        Task<SimpleResult> MarkAsProcessingAsync(Guid storeOrderId);
        Task<SimpleResult> MarkAsShippedAsync(Guid storeOrderId);

        Task<Result<StoreOrderResponse>> GetStoreOrderById(Guid storeOrderId);

        Task<Result<PaginatedResult<StoreOrderResponse>>> GetStoreOrders(OrdersPaginationRequest request);
    }
}
