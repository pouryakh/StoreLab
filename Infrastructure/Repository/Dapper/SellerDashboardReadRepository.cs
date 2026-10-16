using Application.DTOs.SellerDashboardDto;
using Application.DTOs.SellerDashboardDto.Details;
using Application.Interfaces.Repositories.Dapper;
using Dapper;
using Infrastructure.Persistence.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Dapper
{
    public class SellerDashboardReadRepository : ISellerDashboardReadRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SellerDashboardReadRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<SellerDashboardOrdersDetailsResponse> GetOrdersDetails(SellerDashboardOrdersRequest request)
        {
            const string sql =
                """
                SELECT
                    COUNT(
                        CASE
                            WHEN so.Status NOT IN (6, 7)
                            THEN 1
                        END
                    ) AS TotalOrders,

                    SUM(
                        CASE
                            WHEN so.Status = 2
                            THEN 1
                            ELSE 0
                        END
                    ) AS PaidOrders,

                    SUM(
                        CASE
                            WHEN so.Status = 3
                            THEN 1
                            ELSE 0
                        END
                    ) AS ProcessingOrders,

                    SUM(
                        CASE
                            WHEN so.Status = 4
                            THEN 1
                            ELSE 0
                        END
                    ) AS ShippedOrders,

                    SUM(
                        CASE
                            WHEN so.Status = 5
                            THEN 1
                            ELSE 0
                        END
                    ) AS DeliveredOrders,

                    SUM(
                        CASE
                            WHEN so.Status = 6
                            THEN 1
                            ELSE 0
                        END
                    ) AS CancelledOrders,

                    SUM(
                        CASE
                            WHEN so.Status = 7
                            THEN 1
                            ELSE 0
                        END
                    ) AS ReturnedOrders,

                    SUM(
                        CASE
                            WHEN so.Status NOT IN (6, 7)
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS TotalOrderValue,

                    AVG(
                        CASE
                            WHEN so.Status NOT IN (6, 7)
                            THEN so.TotalAmount
                        END
                    ) AS AverageOrderValue,

                    MAX(
                        CASE
                            WHEN so.Status NOT IN (6, 7)
                            THEN so.TotalAmount
                        END
                    ) AS HighestOrderValue

                FROM StoreOrders AS so

                WHERE so.StoreId = @StoreId
                  AND so.CreatedAt >= @FromDate
                  AND so.CreatedAt < DATEADD(DAY, 1, @ToDate);
                """;

            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<SellerDashboardOrdersDetailsResponse>(
                sql,
                new 
                {
                    StoreId = request.StoreId,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate
                });

            return result;
        }

        public async Task<SellerDashboardOverviewResponse> GetOverview(Guid storeId)
        {
            const string sql =
                """
                USE StoreLabDb;

                -- 1. Orders + Turnover
                SELECT
                    COUNT(
                        CASE
                            WHEN so.Status NOT IN (6, 7)
                            THEN 1
                        END
                    ) AS TotalOrders,

                    SUM(
                        CASE
                            WHEN so.Status NOT IN (6, 7)
                             AND so.CreatedAt >= CAST(GETDATE() AS DATE)
                             AND so.CreatedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
                            THEN 1
                            ELSE 0
                        END
                    ) AS TodayOrders,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus NOT IN (4)
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS TotalTurnover,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus NOT IN (4)
                             AND so.CreatedAt >= CAST(GETDATE() AS DATE)
                             AND so.CreatedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS TodayTurnover

                FROM StoreOrders AS so

                WHERE so.StoreId = @StoreId;


                -- 2. Settlement
                SELECT
                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 3
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS TotalCompletedSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 3
                             AND so.SettledAt >= CAST(GETDATE() AS DATE)
                             AND so.SettledAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS TodayCompletedSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 1
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS PendingSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 2
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS EligibleSettlement

                FROM StoreOrders AS so

                WHERE so.StoreId = @StoreId;


                -- 3. Products
                SELECT
                    COUNT(*) AS TotalProducts,

                    SUM(
                        CASE
                            WHEN p.IsActive = 1
                            THEN 1
                            ELSE 0
                        END
                    ) AS ActiveProducts,

                    SUM(
                        CASE
                            WHEN p.StockQuantity = 0
                            THEN 1
                            ELSE 0
                        END
                    ) AS OutOfStockProducts,

                    SUM(
                        CASE
                            WHEN p.StockQuantity > 0
                             AND p.StockQuantity < 10
                            THEN 1
                            ELSE 0
                        END
                    ) AS LowStockProducts

                FROM Products AS p

                WHERE p.StoreId = @StoreId;   
                """;

            using var connection = _connectionFactory.CreateConnection();

            var results = await connection.QueryMultipleAsync(
                sql,
                new 
                {
                    StoreId = storeId
                });

            var orders = await results.ReadSingleAsync<OrderOverviewResponse>();

            var products = await results.ReadSingleAsync<ProductOverviewResponse>();

            var Settlement = await results.ReadSingleAsync<SettlementOverviewResponse>();

            var finalResult = new SellerDashboardOverviewResponse()
            {
                Orders = orders,
                Products = products,
                Settlement = Settlement
            };

            return finalResult;
        }

        public async Task<SellerDashboardProductsDetailsResponse> GetProductDetails(Guid storeId)
        {
            const string sql =
                """
                SELECT
                    COUNT(*) AS TotalProducts,

                    SUM(
                        CASE
                            WHEN p.IsActive = 1
                            THEN 1
                            ELSE 0
                        END
                    ) AS ActiveProducts,

                    SUM(
                        CASE
                            WHEN p.IsActive = 0
                            THEN 1
                            ELSE 0
                        END
                    ) AS InactiveProducts,

                    SUM(
                        CASE
                            WHEN p.StockQuantity = 0
                            THEN 1
                            ELSE 0
                        END
                    ) AS OutOfStockProducts,

                    SUM(
                        CASE
                            WHEN p.StockQuantity > 0
                             AND p.StockQuantity < 10
                            THEN 1
                            ELSE 0
                        END
                    ) AS LowStockProducts,

                    AVG(p.Price) AS AverageProductPrice,
                    MAX(p.Price) AS HighestProductPrice,
                    MIN(p.Price) AS LowestProductPrice

                FROM Products AS p
                WHERE p.StoreId = @StoreId;
                """;
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<SellerDashboardProductsDetailsResponse>(
                sql,
                new
                {
                    StoreId = storeId
                });

            return result;
        }

        public async Task<SellerDashboardSettlementDetailsResponse> GetSettlementDetails(Guid storeId)
        {
            const string sql =
                """
                SELECT
                    -- Pending
                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 1
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS PendingSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 1
                            THEN 1
                            ELSE 0
                        END
                    ) AS PendingCount,

                    -- Eligible
                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 2
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS EligibleSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 2
                            THEN 1
                            ELSE 0
                        END
                    ) AS EligibleCount,

                    -- Completed
                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 3
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS CompletedSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 3
                            THEN 1
                            ELSE 0
                        END
                    ) AS CompletedCount,

                    -- Blocked
                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 4
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS BlockedSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 4
                            THEN 1
                            ELSE 0
                        END
                    ) AS BlockedCount

                FROM StoreOrders AS so

                WHERE so.StoreId = @StoreId;
                """;

            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<SellerDashboardSettlementDetailsResponse>(
                sql,
                new 
                {
                    StoreId = storeId
                });

            return result;
        }
        public async Task<SellerDashboardTurnoverDetailsResponse> GetTurnoverDetails(SellerDashboardTurnoverRequest request)
        {
            const string sql =
                """
                SELECT
                    SUM(
                        CASE
                            WHEN so.Status NOT IN (6, 7, 1)
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS TotalSales,

                    AVG(
                        CASE
                            WHEN so.Status NOT IN (6, 7, 1)
                            THEN so.TotalAmount
                        END
                    ) AS AverageOrderValue,

                    MAX(
                        CASE
                            WHEN so.Status NOT IN (6, 7, 1)
                            THEN so.TotalAmount
                        END
                    ) AS HighestOrderValue,

                    MIN(
                        CASE
                            WHEN so.Status NOT IN (6, 7, 1)
                            THEN so.TotalAmount
                        END
                    ) AS LowestOrderValue,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 3
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS CompletedSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 2
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS EligibleSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 1
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS PendingSettlement,

                    SUM(
                        CASE
                            WHEN so.SettlementStatus = 4
                            THEN so.TotalAmount
                            ELSE 0
                        END
                    ) AS BlockedSettlement

                FROM StoreOrders AS so

                WHERE so.StoreId = @StoreId
                  AND so.CreatedAt >= @FromDate
                  AND so.CreatedAt < DATEADD(DAY, 1, @ToDate);
                """;

            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<SellerDashboardTurnoverDetailsResponse>(
                sql,
                new 
                {
                    StoreId = request.StoreId,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate
                });

            return result;
        }
    }
}
