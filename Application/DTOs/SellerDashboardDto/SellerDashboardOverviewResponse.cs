public class SellerDashboardOverviewResponse
{
    public OrderOverviewResponse Orders { get; set; } = null!;
    public SettlementOverviewResponse Settlement { get; set; } = null!;
    public ProductOverviewResponse Products { get; set; } = null!;
}