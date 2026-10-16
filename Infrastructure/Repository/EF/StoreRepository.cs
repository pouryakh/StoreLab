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
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        public StoreRepository(StoreLabDbContext context) : base(context)
        {
        }

        public async Task<bool> IsNameAlreadyExistsAsync(string name)
        {
            return await _dbSet.AnyAsync(x => x.Name == name.ToLower());
        }

        public async Task<bool> IsOwnerOrNot(Guid userId, Guid storeId)
        {
            return await _dbSet.AnyAsync(x => x.SellerId == userId && x.Id == storeId);
        }

        public override async Task<Store?> GetByIdAsync(Guid id)
        {
            return await _dbSet.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> IsStoreExsistsByIdAsync(Guid storeId)
        {
            return await _dbSet.AnyAsync(x => x.Id == storeId);
        }

        public async Task<List<Store>> GetUserAllStoresAsync(Guid userId)
        {
            return await _dbSet.Where(x => x.SellerId == userId).ToListAsync();
        }

        public async Task<bool> IsOwnerOfStoreOrder(Guid storeOrderId,Guid userId)
        {
            return await _dbSet.AnyAsync(store =>store.SellerId == userId && store.Orders.Any(order => order.Id == storeOrderId));
        }
    }
}
