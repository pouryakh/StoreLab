using Application.Common.Helpers;
using Application.DTOs.OrderDto;
using Application.Interfaces.Repositories.Dapper;
using Application.Results;
using Dapper;
using Infrastructure.Persistence.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Dapper
{
    public class OrderReadRepository : IOrderReadRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderReadRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(Guid userId, Guid orderId)
        {
            using var connection =  _connectionFactory.CreateConnection();

            const string sql =
                """
                SELECT
                    o.Id AS OrderId,
                    o.TotalAmount AS OrderTotalAmount,
                    o.CreatedAt AS OrderCreatedAt,

                    so.Id AS StoreOrderId,
                    so.StoreId,
                    s.Name AS StoreName,
                    so.TotalAmount AS StoreOrderTotalAmount,
                    so.Status AS StoreOrderStatus,
                    so.DeliveredAt,

                    i.ProductId,
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice,

                    p.Id AS PaymentId,
                    p.Amount AS PaymentAmount,
                    p.PaidAt,
                    p.Status AS PaymentStatus,
                    p.OrderId AS PaymentOrderId

                FROM Orders o

                INNER JOIN StoreOrders so
                    ON so.OrderId = o.Id

                INNER JOIN OrderItems i
                    ON i.StoreOrderId = so.Id

                INNER JOIN store s
                    ON s.Id = so.StoreId

                LEFT JOIN Payments p
                    ON p.OrderId = o.Id

                WHERE o.Id = @OrderId and o.UserId = @UserId;
                """;

            var rows = (await connection.QueryAsync<OrderQueryRow>(
                sql,
                new 
                {
                    UserId = userId,
                    OrderId = orderId,
                })).ToList();

            var firstRow = rows.FirstOrDefault();
            if (firstRow is null)
            {
                return null;
            }

            var orderResponse = new OrderResponse()
            {
                CreatedAt = firstRow.OrderCreatedAt,
                OrderId = firstRow.OrderId,
                TotalAmount = firstRow.OrderTotalAmount,
                UserId = userId,
            };

            if (firstRow.PaymentId != null && firstRow.PaymentStatus != null && firstRow.PaymentAmount != null)
            {
                orderResponse.Payment = new PaymentResponse()
                {
                    OrderId = orderId,
                    Amount = firstRow.PaymentAmount.Value,
                    PaidAt = firstRow.PaidAt,
                    PaymentId = firstRow.PaymentId.Value,
                    Status = firstRow.PaymentStatus.Value
                };
            }

            foreach (var group in rows.GroupBy(x => x.StoreOrderId)) 
            {
                var storeOrder = new UserStoreOrderResponse
                {
                    StoreId = group.First().StoreId,
                    StoreOrderId = group.First().StoreOrderId,
                    StoreName = group.First().StoreName,
                    OrderId = group.First().OrderId,
                    Status = group.First().StoreOrderStatus,
                    DeliveredAt = group.First().DeliveredAt,
                    TotalAmount = group.First().StoreOrderTotalAmount,
                    Items = group.Select(x => new OrderItemResponse
                    {
                        ProductId = x.ProductId,
                        ProductName = x.ProductName,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice
                    }).ToList(),
                };

                orderResponse.StoreOrders.Add(storeOrder);

            }

            orderResponse.Status = OrderStatusHelper.Calculate(rows.Select(x => x.StoreOrderStatus));

            return orderResponse;

        }



        public async Task<PaginatedResult<OrderResponse>> GetOrdersAsync(Guid userId,OrdersPaginationRequest request)
        {
            if (request.PageNumber < 1)
                request.PageNumber = 1;

            if (request.PageSize < 1)
                request.PageSize = 10;

            var offset =
                (request.PageNumber - 1) * request.PageSize;

            using var connection =
                _connectionFactory.CreateConnection();

            const string sql =
                """
                WITH PagedOrders AS
                (
                    SELECT
                        o.Id,
                        o.UserId,
                        o.TotalAmount,
                        o.RefundAmount,
                        o.CreatedAt,

                        COUNT(*) OVER() AS TotalCount

                    FROM Orders o

                    WHERE o.UserId = @UserId
                      AND (@MinAmount IS NULL OR o.TotalAmount >= @MinAmount)
                      AND (@MaxAmount IS NULL OR o.TotalAmount <= @MaxAmount)
                      AND (@MinDate IS NULL OR o.CreatedAt >= @MinDate)
                      AND (@MaxDate IS NULL OR o.CreatedAt <= @MaxDate)

                    ORDER BY o.CreatedAt DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                )

                SELECT
                    po.Id AS OrderId,
                    po.TotalAmount AS OrderTotalAmount,
                    po.RefundAmount AS OrderRefundAmount,
                    po.CreatedAt AS OrderCreatedAt,
                    po.TotalCount,

                    so.Id AS StoreOrderId,
                    so.StoreId,
                    s.Name AS StoreName,
                    so.TotalAmount AS StoreOrderTotalAmount,
                    so.Status AS StoreOrderStatus,
                    so.DeliveredAt,

                    i.ProductId,
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice,

                    p.Id AS PaymentId,
                    p.Amount AS PaymentAmount,
                    p.PaidAt,
                    p.Status AS PaymentStatus,
                    p.OrderId AS PaymentOrderId

                FROM PagedOrders po

                INNER JOIN StoreOrders so
                    ON so.OrderId = po.Id

                INNER JOIN OrderItems i
                    ON i.StoreOrderId = so.Id

                INNER JOIN store s
                    ON s.Id = so.StoreId

                LEFT JOIN Payments p
                    ON p.OrderId = po.Id

                ORDER BY po.CreatedAt DESC;
                """;

            var rows = (
                await connection.QueryAsync<OrderQueryRow>(
                    sql,
                    new
                    {
                        UserId = userId,
                        MinAmount = request.MinAmount,
                        MaxAmount = request.MaxAmount,
                        MinDate = request.MinDate,
                        MaxDate = request.MaxDate,
                        PageSize = request.PageSize,
                        Offset = offset
                    })).ToList();


            var paginationResult = new PaginatedResult<OrderResponse>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = rows.FirstOrDefault()?.TotalCount ?? 0
            };

            if (paginationResult.TotalCount == 0)
            {
                return paginationResult;
            }

            foreach (var orderGroup in rows.GroupBy(x => x.OrderId))
            {
                var firstRow = orderGroup.First();

                var orderResponse = new OrderResponse
                {
                    OrderId = orderGroup.Key,
                    CreatedAt = firstRow.OrderCreatedAt,
                    UserId = userId,
                    TotalAmount = firstRow.OrderTotalAmount,
                    RefundAmount = firstRow.OrderRefundAmount,

                };

                if (firstRow.PaymentId.HasValue)
                {
                    orderResponse.Payment = new PaymentResponse
                    {
                        PaymentId = firstRow.PaymentId.Value,
                        OrderId = orderGroup.Key,
                        Amount = firstRow.PaymentAmount!.Value,
                        PaidAt = firstRow.PaidAt,
                        Status = firstRow.PaymentStatus!.Value
                    };
                }

                foreach (var storeOrderGroup in
                         orderGroup.GroupBy(x => x.StoreOrderId))
                {
                    var storeOrderFirstRow = storeOrderGroup.First();

                    var storeOrder = new UserStoreOrderResponse
                    {
                        OrderId = orderGroup.Key,
                        StoreId = storeOrderFirstRow.StoreId,
                        StoreOrderId = storeOrderFirstRow.StoreOrderId,
                        StoreName = storeOrderFirstRow.StoreName,
                        TotalAmount = storeOrderFirstRow.StoreOrderTotalAmount,
                        Status = storeOrderFirstRow.StoreOrderStatus,
                        DeliveredAt = storeOrderFirstRow.DeliveredAt,

                        Items = storeOrderGroup
                            .Select(x => new OrderItemResponse
                            {
                                ProductId = x.ProductId,
                                ProductName = x.ProductName,
                                Quantity = x.Quantity,
                                UnitPrice = x.UnitPrice
                            })
                            .ToList()
                    };

                    orderResponse.StoreOrders.Add(storeOrder);
                }

                orderResponse.Status =
                    OrderStatusHelper.Calculate(
                        orderGroup.Select(x => x.StoreOrderStatus));

                paginationResult.Items.Add(orderResponse);
            }

            return paginationResult;
        }
    }
}
