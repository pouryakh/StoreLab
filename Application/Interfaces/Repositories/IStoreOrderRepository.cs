using Application.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IStoreOrderRepository : IGenericRepository<StoreOrder>
    {
        Task<List<StoreOrder>> GetPendingOrdersReadyForSettlementAsync();

        Task<List<StoreOrder>> GetEligibleOrdersForSettlementAsync();

        Task<List<StoreOrder>> GetShippedOrdersAsync();

        Task<bool> IsStoreOrderOwnedBySeller(Guid storeOrderId,Guid userId);

        Task<bool> BelongsToUserAsync(Guid storeOrderId, Guid userId);
    }
}
