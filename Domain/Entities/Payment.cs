using Domain.Common.Exceptions;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Payment : BaseEntitiy
    {
        public Guid OrderId { get; private set; }

        public decimal Amount { get; private set; }

        public PaymentStatus Status { get; private set; }

        public DateTime? PaidAt { get; private set; }

        public Order Order { get; private set; } = null!;

        private Payment() 
        {
        }

        public Payment(Guid orderId, decimal amount)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("Order is required.");

            if (amount <= 0)
                throw new DomainException("Payment amount must be greater than zero.");

            OrderId = orderId;
            Amount = amount;
            Status = PaymentStatus.Pending;
        }

        public void MarkAsSuccessful()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException("Invalid payment transition.");

            Status = PaymentStatus.Successful;
            PaidAt = DateTime.UtcNow;
        }

        public void MarkAsFailed()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException("Invalid payment transition.");

            Status = PaymentStatus.Failed;
        }

        public void MarkAsRefunded()
        {
            if (Status != PaymentStatus.Successful)
                throw new DomainException("Only successful payments can be refunded.");

            Status = PaymentStatus.Refunded;
        }
    }
}
