using Application.DTOs.ActivationDto;
using Application.DTOs.CommonDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IStoreActivationRequestService
    {
        Task<SimpleResult> ApproveActivationRequest(Guid activationRequestId,string? note);
        Task<SimpleResult> RejectActivationRequest(Guid activationRequestId,string? note);
        Task<Result<PaginatedResult<ActivationResponse>>> GetPendingActivationRequests(PaginationRequest request);
    }
}
