public class SettlementOverviewResponse
{
    public decimal TotalCompletedSettlement { get; set; }
    public decimal TodayCompletedSettlement { get; set; }
    public decimal PendingSettlement { get; set; }
    public decimal EligibleSettlement { get; set; }
}