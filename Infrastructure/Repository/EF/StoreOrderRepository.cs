using Application.Interfaces.Repositories;
using Application.Results;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.EF
{
    public class StoreOrderRepository : GenericRepository<StoreOrder>, IStoreOrderRepository
    {
        public StoreOrderRepository(StoreLabDbContext context) : base(context)
        {
        }

        public override Task<StoreOrder?> GetByIdAsync(Guid id)
        {
            return _dbSet.Include(x => x.Order).Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> BelongsToUserAsync(Guid storeOrderId,Guid userId)
        {
            return await _dbSet
                .AnyAsync(x =>
                    x.Id == storeOrderId &&
                    x.Order.UserId == userId);
        }

        public async Task<List<StoreOrder>> GetEligibleOrdersForSettlementAsync()
        {
            return await _dbSet.Where(x => x.SettlementStatus == Domain.Enums.SettlementStatus.Eligible).ToListAsync();
        }

        public async Task<List<StoreOrder>> GetPendingOrdersReadyForSettlementAsync()
        {
            return await _dbSet.Where(
                x => x.SettlementStatus == Domain.Enums.SettlementStatus.Pending &&
                x.SettlementEligibleAt <= DateTime.UtcNow).ToListAsync();
        }

        public async Task<List<StoreOrder>> GetShippedOrdersAsync()
        {
            return await _dbSet.Where(x => x.Status == Domain.Enums.StoreOrderStatus.Shipped).ToListAsync();
        }

        public async Task<bool> IsStoreOrderOwnedBySeller(Guid storeOrderId, Guid userId)
        {
            return await _dbSet.AnyAsync(x => x.Store.SellerId == userId && x.Id == storeOrderId);
        }
    }
}
