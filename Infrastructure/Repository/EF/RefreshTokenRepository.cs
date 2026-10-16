using Application.Interfaces.Repositories;
using Domain.Entities.Auth;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.EF
{
    public class RefreshTokenRepository
    : GenericRepository<RefreshToken>,
      IRefreshTokenRepository
    {
        public RefreshTokenRepository(StoreLabDbContext context)
            : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
        }
    }
}
