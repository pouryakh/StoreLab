public class SellerDashboardTurnoverDetailsResponse
{
    // Sales
    public decimal TotalSales { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal HighestOrderValue { get; set; }
    public decimal LowestOrderValue { get; set; }

    // Settlement
    public decimal CompletedSettlement { get; set; }
    public decimal EligibleSettlement { get; set; }
    public decimal PendingSettlement { get; set; }
    public decimal BlockedSettlement { get; set; }
}