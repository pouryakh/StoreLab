public class AdminFinanceOverviewResponse
{
    public decimal TotalSales { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalCompletedSettlement { get; set; }
    public decimal TotalPendingSettlement { get; set; }
    public decimal TotalEligibleSettlement { get; set; }
    public decimal TotalBlockedSettlement { get; set; }
}