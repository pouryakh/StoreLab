public class AdminDashboardFinanceDetailsResponse
{
    public decimal TotalSales { get; set; }

    public decimal TotalCommission { get; set; }

    public decimal CompletedSettlement { get; set; }
    public int CompletedSettlementCount { get; set; }

    public decimal PendingSettlement { get; set; }
    public int PendingSettlementCount { get; set; }

    public decimal EligibleSettlement { get; set; }
    public int EligibleSettlementCount { get; set; }

    public decimal BlockedSettlement { get; set; }
    public int BlockedSettlementCount { get; set; }
}