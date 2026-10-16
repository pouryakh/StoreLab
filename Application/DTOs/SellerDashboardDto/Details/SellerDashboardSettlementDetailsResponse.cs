namespace Application.DTOs.SellerDashboardDto.Details;
public class SellerDashboardSettlementDetailsResponse
{
    public decimal PendingSettlement { get; set; }
    public decimal EligibleSettlement { get; set; }
    public decimal CompletedSettlement { get; set; }
    public decimal BlockedSettlement { get; set; }

    public int PendingCount { get; set; }
    public int EligibleCount { get; set; }
    public int CompletedCount { get; set; }
    public int BlockedCount { get; set; }
}