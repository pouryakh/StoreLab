using Application.DTOs.ActivationDto;
using Application.DTOs.CommonDto;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.Dapper
{
    public interface IActivationRequestReadRepository
    {
        Task<IEnumerable<ActivationRequestQueryRow>> GetPendingActivationRequestsAsync(PaginationRequest request);
    }
}
