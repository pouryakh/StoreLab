
using Application.Interfaces.Repositories.Dapper;
using Dapper;
using Infrastructure.Persistence.Connection;

namespace Infrastructure.Repository.Dapper
{
    public class AdminDashboardReadRepository : IAdminDashboardReadRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AdminDashboardReadRepository(
            IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public async Task<AdminDashboardFinanceDetailsResponse> GetAdminDashboardFinanceDetailsAsync()
        {
            const string sql =
                """
                SELECT
                    COALESCE(SUM(CASE
                        WHEN Status NOT IN (6, 7, 1)
                        THEN TotalAmount ELSE 0
                    END), 0) AS TotalSales,

                    COALESCE(SUM(CASE
                        WHEN SettlementStatus = 3
                        THEN TotalAmount * 0.10 ELSE 0
                    END), 0) AS TotalCommission,

                    COALESCE(SUM(CASE
                        WHEN SettlementStatus = 3
                        THEN TotalAmount ELSE 0
                    END), 0) AS CompletedSettlement,

                    COUNT(CASE
                        WHEN SettlementStatus = 3 THEN 1
                    END) AS CompletedSettlementCount,

                    COALESCE(SUM(CASE
                        WHEN SettlementStatus = 1
                        THEN TotalAmount ELSE 0
                    END), 0) AS PendingSettlement,

                    COUNT(CASE
                        WHEN SettlementStatus = 1 THEN 1
                    END) AS PendingSettlementCount,

                    COALESCE(SUM(CASE
                        WHEN SettlementStatus = 2
                        THEN TotalAmount ELSE 0
                    END), 0) AS EligibleSettlement,

                    COUNT(CASE
                        WHEN SettlementStatus = 2 THEN 1
                    END) AS EligibleSettlementCount,

                    COALESCE(SUM(CASE
                        WHEN SettlementStatus = 4
                        THEN TotalAmount ELSE 0
                    END), 0) AS BlockedSettlement,

                    COUNT(CASE
                        WHEN SettlementStatus = 4 THEN 1
                    END) AS BlockedSettlementCount

                FROM StoreOrders;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection
                .QuerySingleAsync<AdminDashboardFinanceDetailsResponse>(sql);
        }

        public async Task<AdminDashboardStoresDetailsResponse> GetAdminDashboardStoresDetailsAsync()
        {
            const string sql =
                """
                -- 1. Stores
                SELECT
                    COUNT(*) AS TotalStores,

                    COALESCE(SUM(CASE
                        WHEN IsActive = 1 THEN 1 ELSE 0
                    END), 0) AS ActiveStores,

                    COALESCE(SUM(CASE
                        WHEN IsActive = 0 THEN 1 ELSE 0
                    END), 0) AS InactiveStores,

                    COALESCE(SUM(CASE
                        WHEN Approval = 2 THEN 1 ELSE 0
                    END), 0) AS PendingApprovalStores,

                    COALESCE(SUM(CASE
                        WHEN Approval = 3 THEN 1 ELSE 0
                    END), 0) AS ApprovedStores,

                    COALESCE(SUM(CASE
                        WHEN Approval = 4 THEN 1 ELSE 0
                    END), 0) AS RejectedStores

                FROM store;

                -- 2. Products
                SELECT
                    COUNT(*) AS TotalProducts,

                    COALESCE(SUM(CASE
                        WHEN IsActive = 1 THEN 1 ELSE 0
                    END), 0) AS ActiveProducts

                FROM Products;
                """;

            using var connection = _connectionFactory.CreateConnection();
            using var results = await connection.QueryMultipleAsync(sql);

            var stores =
                await results.ReadSingleAsync<AdminDashboardStoresDetailsResponse>();

            var products =
                await results.ReadSingleAsync<AdminDashboardStoresDetailsResponse>();

            stores.TotalProducts = products.TotalProducts;
            stores.ActiveProducts = products.ActiveProducts;

            return stores;
        }

        public async Task<AdminDashboardOverviewResponse>  GetAdminDashboardOverviewAsync()
        {
            const string sql =
                """
                -- 1. Users
                SELECT
                    COUNT(*) AS TotalUsers
                FROM Users;

                -- 2. Stores
                SELECT
                    COUNT(*) AS TotalStores,

                    COALESCE(SUM(CASE
                        WHEN IsActive = 1 THEN 1 ELSE 0
                    END), 0) AS ActiveStores
                FROM store;

                -- 3. Orders
                SELECT
                    COUNT(*) AS TotalOrders
                FROM Orders;

                -- 4. Finance
                SELECT
                    COALESCE(SUM(TotalAmount), 0) AS TotalSales,
                    COALESCE(MAX(TotalAmount), 0) AS HighestOrderValue,
                    COALESCE(MIN(TotalAmount), 0) AS LowestOrderValue
                FROM StoreOrders;
                """;

            using var connection = _connectionFactory.CreateConnection();
            using var results = await connection.QueryMultipleAsync(sql);

            var users =
                await results.ReadSingleAsync<AdminUsersOverviewResponse>();

            var stores =
                await results.ReadSingleAsync<AdminStoresOverviewResponse>();

            var orders =
                await results.ReadSingleAsync<AdminOrdersOverviewResponse>();

            var finance =
                await results.ReadSingleAsync<AdminFinanceOverviewResponse>();

            return new AdminDashboardOverviewResponse
            {
                Users = users,
                Stores = stores,
                Orders = orders,
                Finance = finance
            };
        }

        public async Task<AdminDashboardOrdersDetailsResponse> GetAdminDashboardOrederDetailsAsync()
        {
            const string sql =
                """
        -- 1. Orders: تعداد کل سفارش‌های اصلی
        SELECT
            COUNT(*) AS TotalOrders,

            COALESCE(SUM(CASE
                WHEN CreatedAt >= CAST(GETUTCDATE() AS DATE)
                 AND CreatedAt < DATEADD(
                     DAY, 1, CAST(GETUTCDATE() AS DATE))
                THEN 1 ELSE 0
            END), 0) AS OrdersToday

        FROM Orders;


        -- 2. StoreOrders: آمار وضعیت سفارش‌های فروشگاهی
        SELECT
            COALESCE(SUM(CASE
                WHEN Status = 2 THEN 1 ELSE 0
            END), 0) AS PaidOrders,

            COALESCE(SUM(CASE
                WHEN Status = 1 THEN 1 ELSE 0
            END), 0) AS PendingOrders,

            COALESCE(SUM(CASE
                WHEN Status = 3 THEN 1 ELSE 0
            END), 0) AS ProcessingOrders,

            COALESCE(SUM(CASE
                WHEN Status = 4 THEN 1 ELSE 0
            END), 0) AS ShippedOrders,

            COALESCE(SUM(CASE
                WHEN Status = 5 THEN 1 ELSE 0
            END), 0) AS DeliveredOrders,

            COALESCE(SUM(CASE
                WHEN Status = 6 THEN 1 ELSE 0
            END), 0) AS CancelledOrders,

            COALESCE(SUM(CASE
                WHEN Status = 7 THEN 1 ELSE 0
            END), 0) AS ReturnedOrders

        FROM StoreOrders;
        """;

            using var connection = _connectionFactory.CreateConnection();
            using var results = await connection.QueryMultipleAsync(sql);

            var orders =
                await results.ReadSingleAsync<AdminDashboardOrdersDetailsResponse>();

            var storeOrders =
                await results.ReadSingleAsync<AdminDashboardOrdersDetailsResponse>();

            orders.PaidOrders = storeOrders.PaidOrders;
            orders.PendingOrders = storeOrders.PendingOrders;
            orders.ProcessingOrders = storeOrders.ProcessingOrders;
            orders.ShippedOrders = storeOrders.ShippedOrders;
            orders.DeliveredOrders = storeOrders.DeliveredOrders;
            orders.CancelledOrders = storeOrders.CancelledOrders;
            orders.ReturnedOrders = storeOrders.ReturnedOrders;

            return orders;
        }
    }
}
