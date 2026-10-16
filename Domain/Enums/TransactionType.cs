using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdrawal = 2,
        Payment = 3,
        Refund = 4,
        SellerSettlement = 5,
        PlatformCommission = 6
    }
}
