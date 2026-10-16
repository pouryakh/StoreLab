public class AdminDashboardOverviewResponse
{
    public AdminUsersOverviewResponse Users { get; set; } = null!;
    public AdminStoresOverviewResponse Stores { get; set; } = null!;
    public AdminOrdersOverviewResponse Orders { get; set; } = null!;
    public AdminFinanceOverviewResponse Finance { get; set; } = null!;
}