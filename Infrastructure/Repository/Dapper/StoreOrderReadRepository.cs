using Application.DTOs.OrderDto;
using Application.Interfaces.Repositories.Dapper;
using Application.Results;
using Dapper;
using Infrastructure.Persistence.Connection;

namespace Infrastructure.Repository.Dapper
{
    public class StoreOrderReadRepository : IStoreOrderReadRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StoreOrderReadRepository(
            IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<StoreOrderResponse?> GetStoreOrderByIdAsync(
            Guid userId,
            Guid storeOrderId)
        {
            const string sql =
                """
                SELECT
                    so.Id AS StoreOrderId,
                    so.StoreId,
                    so.TotalAmount,
                    so.Status,
                    so.SettlementStatus,
                    so.SettlementEligibleAt,
                    so.SettledAt,
                    so.DeliveredAt,
                    so.EstimatedDeliveryAt,
                    so.CreatedAt,

                    i.ProductId,
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice

                FROM StoreOrders so

                INNER JOIN Stores s
                    ON s.Id = so.StoreId

                INNER JOIN OrderItems i
                    ON i.StoreOrderId = so.Id

                WHERE so.Id = @StoreOrderId
                  AND s.SellerId = @UserId;
                """;

            using var connection =
                _connectionFactory.CreateConnection();

            var rows =
                (await connection.QueryAsync<StoreOrderQueryRow>(
                    sql,
                    new
                    {
                        UserId = userId,
                        StoreOrderId = storeOrderId
                    }))
                .ToList();

            if (!rows.Any())
                return null;

            var firstRow = rows.First();

            var response = new StoreOrderResponse
            {
                CreatedAt = firstRow.CreatedAt,
                DeliveredAt = firstRow.DeliveredAt,
                EstimatedDeliveryAt = firstRow.EstimatedDeliveryAt,
                SettledAt = firstRow.SettledAt,
                SettlementEligibleAt = firstRow.SettlementEligibleAt,
                SettlementStatus = firstRow.SettlementStatus,
                Status = firstRow.Status,
                StoreId = firstRow.StoreId,
                StoreOrderId = firstRow.StoreOrderId,
                TotalAmount = firstRow.TotalAmount,

                Items = rows.Select(x => new OrderItemResponse
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                }).ToList()
            };

            return response;
        }

        public async Task<PaginatedResult<StoreOrderResponse>> GetStoreOrdersAsync(Guid userId, OrdersPaginationRequest request)
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
                WITH PagedStoreOrders AS
                (
                    SELECT
                        so.Id AS StoreOrderId,
                        so.StoreId,
                        so.TotalAmount,
                        so.Status,
                        so.SettlementStatus,
                        so.SettlementEligibleAt,
                        so.SettledAt,
                        so.DeliveredAt,
                        so.EstimatedDeliveryAt,
                        so.CreatedAt,

                        COUNT(*) OVER() AS TotalCount

                    FROM StoreOrders so

                    INNER JOIN Store s
                        ON s.Id = so.StoreId

                    WHERE s.SellerId = @UserId
                      AND (@MinAmount IS NULL OR so.TotalAmount >= @MinAmount)
                      AND (@MaxAmount IS NULL OR so.TotalAmount <= @MaxAmount)
                      AND (@MinDate IS NULL OR so.CreatedAt >= @MinDate)
                      AND (@MaxDate IS NULL OR so.CreatedAt <= @MaxDate)

                    ORDER BY so.CreatedAt DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                )

                SELECT
                    so.StoreOrderId,
                    so.StoreId,
                    so.TotalAmount,
                    so.Status,
                    so.SettlementStatus,
                    so.SettlementEligibleAt,
                    so.SettledAt,
                    so.DeliveredAt,
                    so.EstimatedDeliveryAt,
                    so.CreatedAt,
                    so.TotalCount,

                    i.ProductId,
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice

                FROM PagedStoreOrders so

                INNER JOIN OrderItems i
                    ON i.StoreOrderId = so.StoreOrderId

                ORDER BY so.CreatedAt DESC;
                """;

            var rows =
                (await connection.QueryAsync<StoreOrderQueryRow>(
                    sql,
                    new
                    {
                        UserId = userId,
                        MinAmount = request.MinAmount,
                        MaxAmount = request.MaxAmount,
                        MinDate = request.MinDate,
                        MaxDate = request.MaxDate,
                        Offset = offset,
                        PageSize = request.PageSize
                    }))
                .ToList();

            if (!rows.Any())
            {
                return new PaginatedResult<StoreOrderResponse>
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = 0
                };
            }

            var result = new PaginatedResult<StoreOrderResponse>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = rows.First().TotalCount
            };

            foreach (var group in rows.GroupBy(x => x.StoreOrderId))
            {
                var firstRow = group.First();

                var storeOrderResponse = new StoreOrderResponse
                {
                    CreatedAt = firstRow.CreatedAt,
                    DeliveredAt = firstRow.DeliveredAt,
                    StoreOrderId = group.Key,
                    EstimatedDeliveryAt = firstRow.EstimatedDeliveryAt,
                    SettledAt = firstRow.SettledAt,
                    SettlementEligibleAt = firstRow.SettlementEligibleAt,
                    SettlementStatus = firstRow.SettlementStatus,
                    Status = firstRow.Status,
                    StoreId = firstRow.StoreId,
                    TotalAmount = firstRow.TotalAmount
                };

                foreach (var item in group)
                {
                    var storeOrderItemResponse = new OrderItemResponse
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };

                    storeOrderResponse.Items.Add(storeOrderItemResponse);
                }

                result.Items.Add(storeOrderResponse);
            }

            return result;
        }
    }
}