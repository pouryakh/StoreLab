using Application.DTOs.ReturnRequestDto;
using Application.Interfaces.Repositories.Dapper;
using Application.Results;
using Dapper;
using Infrastructure.Persistence.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Dapper
{
    public class ReturnRequestReadRepository : IReturnRequestReadRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ReturnRequestReadRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PaginatedResult<ReturnRequestRsponse>> GetAllReturnRequestAsync(ReturnRequestPaginationRequest request)
        {
            var offset = (request.PageNumber - 1) * request.PageSize;

            const string sql =
                """
                SELECT
                    rr.Id,
                    rr.Reason,
                    rr.ReceivedAt,
                    rr.RequestedAt,
                    rr.ResolvedAt,
                    rr.Status,
                    rr.StoreOrderId
                FROM ReturnRequests AS rr
                WHERE (@FromDate IS NULL OR rr.RequestedAt >= @FromDate)
                  AND (@ToDate IS NULL OR rr.RequestedAt < DATEADD(DAY, 1, @ToDate))
                  AND (@ReturnStatus IS NULL OR rr.Status = @ReturnStatus)
                ORDER BY rr.RequestedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM ReturnRequests AS rr
                WHERE (@FromDate IS NULL OR rr.RequestedAt >= @FromDate)
                  AND (@ToDate IS NULL OR rr.RequestedAt < DATEADD(DAY, 1, @ToDate))
                  AND (@ReturnStatus IS NULL OR rr.Status = @ReturnStatus);
                """;

            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                request.FromDate,
                request.ToDate,
                ReturnStatus = request.ReturnStatus.HasValue
                    ? (int?)request.ReturnStatus.Value
                    : null,
                Offset = offset,
                request.PageSize
            });

            var items = (await multi.ReadAsync<ReturnRequestRsponse>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new PaginatedResult<ReturnRequestRsponse>() 
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
                
        }

        public async Task<PaginatedResult<ReturnRequestRsponse>> GetAllStoreReturnRequestsAsync(
    ReturnRequestPaginationRequest request,
    Guid sellerId)
        {
            var offset = (request.PageNumber - 1) * request.PageSize;

            const string sql =
                """
                SELECT
                    rr.Id,
                    rr.Reason,
                    rr.ReceivedAt,
                    rr.RequestedAt,
                    rr.ResolvedAt,
                    rr.Status,
                    rr.StoreOrderId
                FROM ReturnRequests AS rr
                INNER JOIN StoreOrders AS so
                    ON rr.StoreOrderId = so.Id
                INNER JOIN Store AS s
                    ON so.StoreId = s.Id
                WHERE s.SellerId = @SellerId
                    AND (@FromDate IS NULL OR rr.RequestedAt >= @FromDate)
                    AND (@ToDate IS NULL OR rr.RequestedAt < DATEADD(DAY, 1, @ToDate))
                    AND (@ReturnStatus IS NULL OR rr.Status = @ReturnStatus)
                ORDER BY rr.RequestedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM ReturnRequests AS rr
                INNER JOIN StoreOrders AS so
                    ON rr.StoreOrderId = so.Id
                INNER JOIN Store AS s
                    ON so.StoreId = s.Id
                WHERE s.SellerId = @SellerId
                    AND (@FromDate IS NULL OR rr.RequestedAt >= @FromDate)
                    AND (@ToDate IS NULL OR rr.RequestedAt < DATEADD(DAY, 1, @ToDate))
                    AND (@ReturnStatus IS NULL OR rr.Status = @ReturnStatus);
                """;

            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                SellerId = sellerId,
                request.FromDate,
                request.ToDate,
                ReturnStatus = request.ReturnStatus.HasValue
                    ? (int?)request.ReturnStatus.Value
                    : null,
                Offset = offset,
                request.PageSize
            });

            var items = (await multi.ReadAsync<ReturnRequestRsponse>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new PaginatedResult<ReturnRequestRsponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResult<ReturnRequestRsponse>> GetAllUserReturnRequestsAsync(ReturnRequestPaginationRequest request, Guid userId)
        {
            var offset = (request.PageNumber - 1) * request.PageSize;

            const string sql =
                """
                SELECT
                   rr.Id,
                   rr.Reason,
                   rr.ReceivedAt,
                   rr.RequestedAt,
                   rr.ResolvedAt,
                   rr.Status,
                   rr.StoreOrderId
                FROM ReturnRequests AS rr
                WHERE rr.UserId = @UserId
                  AND (@FromDate IS NULL OR rr.RequestedAt >= @FromDate)
                  AND (@ToDate IS NULL OR rr.RequestedAt < DATEADD(DAY, 1, @ToDate))
                  AND (@ReturnStatus IS NULL OR rr.Status = @ReturnStatus)
                ORDER BY rr.RequestedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                   SELECT COUNT(*)
                   FROM ReturnRequests AS rr
                   WHERE rr.UserId = @UserId
                     AND (@FromDate IS NULL OR rr.RequestedAt >= @FromDate)
                     AND (@ToDate IS NULL OR rr.RequestedAt < DATEADD(DAY, 1, @ToDate))
                     AND (@ReturnStatus IS NULL OR rr.Status = @ReturnStatus);
                """;

            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                UserId = userId,
                request.FromDate,
                request.ToDate,
                ReturnStatus = request.ReturnStatus.HasValue
                    ? (int?)request.ReturnStatus.Value
                    : null,
                Offset = offset,
                request.PageSize
            });

            var items = (await multi.ReadAsync<ReturnRequestRsponse>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new PaginatedResult<ReturnRequestRsponse>()
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
