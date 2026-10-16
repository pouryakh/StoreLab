using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IWalletRepository : IGenericRepository<Wallet>
    {
        Task<Wallet?> GetWalletByUserId(Guid userId);
        Task<Wallet?> GetEscrowWalletAsync();

        Task<Wallet?> GetPlatformWalletAsync();
        Task<Wallet?> GetSellerWalletByStoreIdAsync(Guid StoreId);
    }
}
