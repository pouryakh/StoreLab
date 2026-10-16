using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ReturnRequestDto
{
    public class CreateReturnRequestDto
    {
        public Guid StoreOrderId { get; set; }

        public string Reason { get; set; } = null!;
    }
}
