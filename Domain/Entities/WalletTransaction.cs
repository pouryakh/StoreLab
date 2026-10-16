using Domain.Common.Exceptions;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WalletTransaction : BaseEntitiy
    {
        public Guid WalletId { get; private set; }

        public decimal Amount { get; private set; }

        public TransactionType Type { get; private set; }

        public Guid? OrderId { get; private set; }

        public string Description { get; private set; } = null!;

        public Guid? ReturnRequestId { get; private set; }

        public ReturnRequest? ReturnRequest { get; private set; }

        public Wallet Wallet { get; private set; } = null!;

        public WalletTransaction(
           Guid walletId,
           decimal amount,
           TransactionType type,
           string? description,
           Guid? orderId = null,
           Guid? returnRequestId = null)
        {
            

            if (amount <= 0)
                throw new DomainException("Transaction amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Transaction description cannot be empty.");

            WalletId = walletId;
            ReturnRequestId = returnRequestId;
            Amount = amount;
            Type = type;
            Description = description.Trim();
            OrderId = orderId;
        }
    }
}
