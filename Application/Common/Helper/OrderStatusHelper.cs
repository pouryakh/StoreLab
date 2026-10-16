using Domain.Enums;

namespace Application.Common.Helpers
{
    public static class OrderStatusHelper
    {
        public static OrderStatus Calculate(
            IEnumerable<StoreOrderStatus> statuses)
        {
            var statusList = statuses.ToList();

            if (statusList.Count == 0)
                return OrderStatus.Pending;

            if (statusList.All(x => x == StoreOrderStatus.Cancelled))
                return OrderStatus.Cancelled;

            if (statusList.All(x => x == StoreOrderStatus.Delivered))
                return OrderStatus.Delivered;

            if (statusList.All(x => x == StoreOrderStatus.Paid))
                return OrderStatus.Paid;

            if (statusList.All(x => x == StoreOrderStatus.Pending))
                return OrderStatus.Pending;

            if (statusList.Any(x => x == StoreOrderStatus.Delivered))
            {
                return OrderStatus.PartiallyDelivered;
            }

            if (statusList.Any(x => x == StoreOrderStatus.Shipped))
            {
                return OrderStatus.PartiallyShipped;
            }

            return OrderStatus.Processing;
        }
    }
}