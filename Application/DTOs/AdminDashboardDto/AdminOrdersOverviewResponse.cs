public class AdminOrdersOverviewResponse
{
    public int TotalOrders { get; set; }
    public int OrdersToday { get; set; }
    public int PaidOrders { get; set; }
    public int PendingOrders { get; set; }
    public int DeliveredOrders { get; set; }
}