using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum TicketStatus
    {
        Open = 1,
        InProgress = 2,
        WaitingForUser = 3,
        Resolved = 4,
        Closed = 5
    }
}
