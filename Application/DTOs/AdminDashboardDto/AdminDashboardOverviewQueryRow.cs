public class AdminDashboardOverviewQueryRow
{
    // Users
    public int TotalUsers { get; set; }
    public int TotalBuyers { get; set; }
    public int TotalSellers { get; set; }
    public int NewUsersToday { get; set; }

    // Stores
    public int TotalStores { get; set; }
    public int ActiveStores { get; set; }
    public int PendingApprovalStores { get; set; }
    public int RejectedStores { get; set; }

    // Orders
    public int TotalOrders { get; set; }
    public int OrdersToday { get; set; }
    public int PaidOrders { get; set; }
    public int PendingOrders { get; set; }
    public int DeliveredOrders { get; set; }

    // Finance
    public decimal TotalSales { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalCompletedSettlement { get; set; }
    public decimal TotalPendingSettlement { get; set; }
    public decimal TotalEligibleSettlement { get; set; }
    public decimal TotalBlockedSettlement { get; set; }
}