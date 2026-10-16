using Application.DTOs.WalletDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IWalletService
    {
        Task<Result<WalletResponse>> GetUserWallet();

        Task<Result<WalletTransactionResponse>> DepositToWallet(DepositRequest request);

        Task<Result<PaginatedResult<WalletTransactionResponse>>> GetTransactions(WalletPaginationRequest request);

        Task<Result<WalletTransactionResponse>> GetTransactionById(Guid transactionId);
    }
}
