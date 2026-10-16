using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WalletDto
{
    public class WalletTransactionResponse
    {
        public Guid WalletId { get; set; }

        public Guid TransactionId { get; set; }

        public decimal Amount { get; set; }

        public TransactionType Type { get; set; }

        public decimal BalanceAfter { get; set; }

        public DateTime createdAt { get; set; }

        public Guid? OrderId { get; set; }

        public string Description { get; set; } = null!;
    }
}
