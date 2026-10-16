public class SellerDashboardOrdersDetailsResponse
{
    // Orders
    public int TotalOrders { get; set; }
    public int PaidOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int CancelledOrders { get; set; }
    public int ReturnedOrders { get; set; }

    // Financial
    public decimal TotalOrderValue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal HighestOrderValue { get; set; }
}