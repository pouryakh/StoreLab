public class AdminDashboardStoresDetailsResponse
{
    public int TotalStores { get; set; }

    public int ActiveStores { get; set; }
    public int InactiveStores { get; set; }

    public int PendingApprovalStores { get; set; }
    public int ApprovedStores { get; set; }
    public int RejectedStores { get; set; }

    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
}