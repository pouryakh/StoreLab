using Application.Interfaces.Repositories;
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
    public class ReturnRequestRepository : GenericRepository<ReturnRequest>, IReturnRequestRepository
    {
        public ReturnRequestRepository(StoreLabDbContext context) : base(context)
        {
        }

        public async Task<List<ReturnRequest>> GetRefundPendingReturnRequestsAsync()
        {
            return await _dbSet
                .Where(x =>  x.Status == ReturnStatus.PendingRefund)
                .Include(x => x.StoreOrder)
                .ToListAsync();
        }

        public async Task<bool> IsAnyReturnRequestWithThisStoreOrderExsist(Guid storeOrderId)
        {
            return await _dbSet.AnyAsync(x => x.StoreOrderId == storeOrderId);
        }
    }
}
