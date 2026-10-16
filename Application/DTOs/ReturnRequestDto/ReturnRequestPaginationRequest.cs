using Application.DTOs.CommonDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ReturnRequestDto
{
    public class ReturnRequestPaginationRequest : PaginationRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public ReturnStatus? ReturnStatus { get; set; }
    }
}
