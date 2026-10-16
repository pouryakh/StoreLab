using Domain.Enums;

public class StoreOrderResponse
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

    public ICollection<OrderItemResponse> Items { get; set; }
        = new List<OrderItemResponse>();
}