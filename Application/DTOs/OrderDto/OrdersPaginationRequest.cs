using Application.DTOs.CommonDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OrderDto
{
    public class OrdersPaginationRequest : PaginationRequest
    {
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public DateTime? MinDate { get; set; } = null;
        public DateTime? MaxDate { get; set; } = null;
    }
}
