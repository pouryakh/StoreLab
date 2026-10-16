using Domain.Enums;

public class PaymentResponse
{
    public Guid PaymentId { get; set; }

    public Guid OrderId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; }

    public DateTime? PaidAt { get; set; }
}