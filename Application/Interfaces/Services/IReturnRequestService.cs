using Application.DTOs.ReturnRequestDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IReturnRequestService
    {
        Task<Result<ReturnRequestRsponse>> CreateReturnRequest(CreateReturnRequestDto request);

        Task<Result<ReturnRequestRsponse>> GetReturnRequest(Guid returnRequestId);

        Task<Result<PaginatedResult<ReturnRequestRsponse>>> GetMyReturnRequest(ReturnRequestPaginationRequest request);

        Task<Result<PaginatedResult<ReturnRequestRsponse>>> GetReturnRequestForAdmins(ReturnRequestPaginationRequest request);
        Task<Result<PaginatedResult<ReturnRequestRsponse>>> GetSellerReturnRequest(ReturnRequestPaginationRequest request);

        Task<SimpleResult> CancelStoreOrderAsync(Guid storeOrderId);

        Task<SimpleResult> MarkAsReturned(Guid returnRequestId);
    }
}
