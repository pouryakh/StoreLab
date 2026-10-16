using Application.DTOs.CommonDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WalletDto
{
    public class WalletPaginationRequest : PaginationRequest
    {
        public DateTime? MaxDate { get; set; }

        public DateTime? MinDate { get; set; }

        public decimal? MaxAmount { get; set; }
        public decimal? MinAmount { get; set; }
    }
}
