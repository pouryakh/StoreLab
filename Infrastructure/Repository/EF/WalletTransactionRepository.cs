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
    public class WalletTransactionRepository : GenericRepository<WalletTransaction>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(StoreLabDbContext context) : base(context)
        {
        }

        public override async Task<WalletTransaction?> GetByIdAsync(Guid id)
        {
            return await _dbSet.Include(x => x.Wallet).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
