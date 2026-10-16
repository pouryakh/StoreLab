using Domain.Enums;

public class OrderQueryRow
{
    public Guid OrderId { get; set; }
    public decimal OrderTotalAmount { get; set; }

    public decimal OrderRefundAmount { get; set; }
    public DateTime OrderCreatedAt { get; set; }

    public Guid StoreOrderId { get; set; }
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = null!;
    public decimal StoreOrderTotalAmount { get; set; }
    public StoreOrderStatus StoreOrderStatus { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Guid? PaymentId { get; set; }
    public decimal? PaymentAmount { get; set; }
    public DateTime? PaidAt { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public Guid? PaymentOrderId { get; set; }

    public int TotalCount { get; set; } = 0;
}