using Application.DTOs.WalletDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface ITransactionReadRepository
    {
        Task<PaginatedResult<WalletTransactionResponse>> ReadPaginationAsync(Guid userId, WalletPaginationRequest request);
    }
}
