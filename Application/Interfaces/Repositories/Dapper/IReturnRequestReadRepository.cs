using Application.DTOs.ReturnRequestDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.Dapper
{
    public interface IReturnRequestReadRepository
    {
        Task<PaginatedResult<ReturnRequestRsponse>> GetAllUserReturnRequestsAsync(ReturnRequestPaginationRequest request,Guid userId);
        Task<PaginatedResult<ReturnRequestRsponse>> GetAllReturnRequestAsync(ReturnRequestPaginationRequest request);
        Task<PaginatedResult<ReturnRequestRsponse>> GetAllStoreReturnRequestsAsync(ReturnRequestPaginationRequest request,Guid sellerId);
    }
}
