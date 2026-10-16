using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Paid = 2,
        Processing = 3,
        PartiallyShipped = 4,
        Shipped = 5,
        PartiallyDelivered = 6,
        Delivered = 7,
        Cancelled = 8
    }
}
