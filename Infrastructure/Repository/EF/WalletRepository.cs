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
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(StoreLabDbContext context) : base(context)
        {
        }

        public async Task<Wallet?> GetEscrowWalletAsync()
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Type == Domain.Enums.WalletType.Escrow);
        }

        public async Task<Wallet?> GetPlatformWalletAsync() 
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Type == Domain.Enums.WalletType.Platform);
        }

        public async Task<Wallet?> GetSellerWalletByStoreIdAsync(Guid StoreId)
        {
            var userId = await _context.Stores.Where(x => x.Id == StoreId).Select(x => x.SellerId).FirstOrDefaultAsync();
            return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Wallet?> GetWalletByUserId(Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
