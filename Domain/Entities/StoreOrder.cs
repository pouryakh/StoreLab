using Domain.Common.Exceptions;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StoreOrder : BaseEntitiy
    {
        public Guid OrderId { get; private set; }
        public Guid StoreId { get; private set; }

        public decimal TotalAmount { get; private set; }

        public StoreOrderStatus Status { get; private set; }
        public SettlementStatus SettlementStatus { get; private set; }

        public DateTime? SettlementEligibleAt { get; private set; }
        public DateTime? SettledAt { get; private set; }

        public DateTime? DeliveredAt { get; private set; }
        public DateTime? EstimatedDeliveryAt { get; private set; }

        public Order Order { get; private set; } = null!;
        public Store Store { get; private set; } = null!;

        public ICollection<OrderItem> Items { get; private set; }
            = new List<OrderItem>();

        public StoreOrder()
        {
        }
        public StoreOrder(Guid orderId,Guid storeId)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("OrderId cannot be empty.");

            if (storeId == Guid.Empty)
                throw new DomainException("StoreId cannot be empty.");

            OrderId = orderId;
            StoreId = storeId;
            
            

            Status = StoreOrderStatus.Pending;
            SettlementStatus = SettlementStatus.Pending;
        }
        public void MarkAsPaid() 
        {
            if (Status != StoreOrderStatus.Pending)
                throw new DomainException("Invalid Transition");

            Status = StoreOrderStatus.Paid;
        }
        public void StartProcessing() 
        {
            if (Status != StoreOrderStatus.Paid)
                throw new DomainException("Invalid Transition");

            Status = StoreOrderStatus.Processing;
        }
        public void MarkAsShipped() 
        {
            if (Status != StoreOrderStatus.Processing)
                throw new DomainException("Invalid Transition");

            EstimatedDeliveryAt = DateTime.UtcNow.AddDays(3);

            Status = StoreOrderStatus.Shipped;
        }
        public void MarkAsDelivered() 
        {
            if (Status != StoreOrderStatus.Shipped)
                throw new DomainException("Invalid Transition");

            Status = StoreOrderStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            SettlementEligibleAt = DeliveredAt.Value.AddDays(7);

            SettlementStatus = SettlementStatus.Pending;

        }
        public void Cancel() 
        {
            if (Status == StoreOrderStatus.Shipped || Status == StoreOrderStatus.Delivered || Status == StoreOrderStatus.Cancelled)
                throw new DomainException("Invalid Transition");

            Status = StoreOrderStatus.Cancelled;
        }

        public void MarkAsEligible()
        {
            if (SettlementStatus != SettlementStatus.Pending)
                return;

            if (SettlementEligibleAt is null || SettlementEligibleAt > DateTime.UtcNow)
                return;

            SettlementStatus = SettlementStatus.Eligible;
        }

        public void CompleteSettlement()
        {
            if (SettlementStatus != SettlementStatus.Eligible)
                throw new DomainException("Invalid Settlement Transition");

            SettlementStatus = SettlementStatus.Completed;
            SettledAt = DateTime.UtcNow;
        }

        public void BlockSettlement() 
        {
            if (SettlementStatus == SettlementStatus.Eligible)
                throw new DomainException("Invalid Settlement Transition");

            SettlementStatus = SettlementStatus.Blocked;
        }

        public void SetItems(List<OrderItem> items)
        {
            if (items is null || items.Count == 0)
                throw new DomainException("Store order must contain at least one item.");

            Items = items;
            TotalAmount = items.Sum(x => x.TotalPrice);
        }
    }
}
