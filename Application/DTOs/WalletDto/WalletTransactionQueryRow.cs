public class WalletTransactionQueryRow
{
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }
    public Guid? OrderId { get; set; }
    public int TotalCount { get; set; }
}