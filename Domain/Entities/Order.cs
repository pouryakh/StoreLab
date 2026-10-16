using Domain.Common.Exceptions;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order : BaseEntitiy
    {
        public Guid UserId { get; private set; }

        public decimal TotalAmount { get; private set; }

        public decimal RefundAmount { get; private set; } = 0;

        public User User { get; private set; } = null!;

        public ICollection<StoreOrder> StoreOrders { get; private set; }
            = new List<StoreOrder>();

        public Payment Payment { get; private set; } = null!;

        public Order()
        {
        }

        public Order(Guid userId,decimal totalAmount) 
        {
            UserId = userId;
            TotalAmount = totalAmount;
        }

        public void ChangeRefundAmount(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Refund amount must be greater than zero.");

            if (RefundAmount + amount > TotalAmount)
                throw new DomainException("Refund amount cannot exceed the order total.");

            RefundAmount += amount;
        }
    }
}
