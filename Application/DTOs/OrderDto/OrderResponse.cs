using Application.DTOs.OrderDto;
using Domain.Enums;

public class OrderResponse
{
    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal RefundAmount { get; set; }
    public decimal NetAmount => TotalAmount - RefundAmount;

    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public PaymentResponse Payment { get; set; } = null!;

    public ICollection<UserStoreOrderResponse> StoreOrders { get; set; }
        = new List<UserStoreOrderResponse>();

}