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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(StoreLabDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<Guid> Ids)
        {
            return await _dbSet
                .Where(x => Ids.Contains(x.Id))
                .ToListAsync();
        }

        public override async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbSet.Include(x => x.Store).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
