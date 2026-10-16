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
    public class StoreActivationRepository : GenericRepository<StoreActivationRequest>, IStoreActivationRepository
    {
        public StoreActivationRepository(StoreLabDbContext context) : base(context)
        {
        }

        public override async Task<StoreActivationRequest?> GetByIdAsync(Guid id)
        {
            return await _dbSet.Include(x => x.Store).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
