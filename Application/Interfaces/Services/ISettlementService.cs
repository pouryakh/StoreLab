using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ISettlementService
    {
        Task<SimpleResult> MarkEligibleOrdersAsync();

        Task<SimpleResult> SettleEligibleOrdersAsync();
    }
}
