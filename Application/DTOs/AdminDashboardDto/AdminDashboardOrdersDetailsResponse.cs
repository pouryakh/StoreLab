public class AdminDashboardOrdersDetailsResponse
{
    public int TotalOrders { get; set; }
    public int OrdersToday { get; set; }

    public int PaidOrders { get; set; }
    public int PendingOrders { get; set; }

    public int ProcessingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public int DeliveredOrders { get; set; }

    public int CancelledOrders { get; set; }
    public int ReturnedOrders { get; set; }
}