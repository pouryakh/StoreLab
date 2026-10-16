using Application.DTOs.OrderDto;
using Domain.Enums;

public class UserStoreOrderResponse
{
    public Guid StoreOrderId { get; set; }
    public Guid OrderId { get; set; }

    public Guid StoreId { get; set; }

    public string StoreName { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public StoreOrderStatus Status { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public ICollection<OrderItemResponse> Items { get; set; }
        = new List<OrderItemResponse>();
}