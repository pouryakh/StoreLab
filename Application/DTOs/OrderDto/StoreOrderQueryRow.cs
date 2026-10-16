using Domain.Enums;

namespace Application.DTOs.OrderDto
{
    public class StoreOrderQueryRow
    {
        public Guid StoreOrderId { get; set; }
        public Guid StoreId { get; set; }

        public decimal TotalAmount { get; set; }

        public StoreOrderStatus Status { get; set; }
        public SettlementStatus SettlementStatus { get; set; }

        public DateTime? SettlementEligibleAt { get; set; }
        public DateTime? SettledAt { get; set; }

        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedDeliveryAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public int TotalCount { get; set; } = 0;
    }
}