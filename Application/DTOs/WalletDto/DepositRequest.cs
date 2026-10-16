using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WalletDto
{
    public class DepositRequest
    {
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}
