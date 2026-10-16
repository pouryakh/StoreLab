using Domain.Common.Exceptions;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Wallet : BaseEntitiy
    {
        public Guid? UserId { get; private set; }

        public WalletType Type { get; private set; }

        public decimal Balance { get; private set; }

        public User? User { get; private set; }

        public ICollection<WalletTransaction> Transactions { get; private set; }
            = new List<WalletTransaction>();

        public Wallet(Guid? userId, WalletType type)
        {
            if ((type == WalletType.Buyer || type == WalletType.Seller)
                && (!userId.HasValue || userId == Guid.Empty))
            {
                throw new DomainException("User is required for this wallet type.");
            }

            if ((type == WalletType.Platform || type == WalletType.Escrow)
                && userId.HasValue)
            {
                throw new DomainException("This wallet type cannot belong to a user.");
            }

            UserId = userId;
            Type = type;
            Balance = 0;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Deposit amount must be greater than zero.");

            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Withdrawal amount must be greater than zero.");

            if (amount > Balance)
                throw new DomainException("Insufficient wallet balance .");

            Balance -= amount;
        }

    }
}
