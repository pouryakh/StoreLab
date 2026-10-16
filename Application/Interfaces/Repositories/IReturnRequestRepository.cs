using Application.Results;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IReturnRequestRepository : IGenericRepository<Domain.Entities.ReturnRequest>
    {
        Task<List<Domain.Entities.ReturnRequest>> GetRefundPendingReturnRequestsAsync();

        Task<bool> IsAnyReturnRequestWithThisStoreOrderExsist(Guid storeOrderId);
    }
}
