using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IStoreRepository : IGenericRepository<Store>
    {
        Task<bool> IsOwnerOrNot(Guid userId, Guid storeId);

        Task<bool> IsStoreExsistsByIdAsync(Guid storeId);

        Task<bool> IsNameAlreadyExistsAsync(string name);

        Task<bool> IsOwnerOfStoreOrder(Guid storeOrderId,Guid userId);

        Task<List<Store>> GetUserAllStoresAsync(Guid userId);
    }
}
