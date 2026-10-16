using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ReturnRequestDto
{
    public class ReturnRequestRsponse
    {
        public Guid Id { get; set; }

        public Guid StoreOrderId { get; set; }

        public string Reason { get; set; } = null!;

        public ReturnStatus Status { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }

    }
}
