using Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CommissionService : ICommissionService
    {
        private decimal commission = 0.10m;
        public decimal CalculateCommission(decimal amount)
        {
            return commission * amount;
        }
    }
}
