using Application.DTOs.WalletDto;
using Application.Interfaces.Repositories;
using Application.Results;
using Dapper;
using Infrastructure.Persistence.Connection;

namespace Infrastructure.Repository.Dapper
{
    public class TransactionReadRepository : ITransactionReadRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TransactionReadRepository(
            IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PaginatedResult<WalletTransactionResponse>> ReadPaginationAsync(
            Guid userId,
            WalletPaginationRequest request)
        {
            var offset =
                (request.PageNumber - 1) * request.PageSize;

            using var connection =
                _connectionFactory.CreateConnection();

            const string sql =
                """
                SELECT
                    t.Amount,
                    t.CreatedAt,
                    t.Description,
                    t.OrderId,
                    COUNT(*) OVER() AS TotalCount
                FROM WalletTransactions t
                INNER JOIN Wallets w
                    ON w.Id = t.WalletId
                WHERE w.UserId = @UserId
                    AND (@MinDate IS NULL OR t.CreatedAt >= @MinDate)
                    AND (@MaxDate IS NULL OR t.CreatedAt <= @MaxDate)
                    AND (@MinAmount IS NULL OR t.Amount >= @MinAmount)
                    AND (@MaxAmount IS NULL OR t.Amount <= @MaxAmount)
                ORDER BY t.CreatedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;
                """;

            var rows =  (await connection.QueryAsync<WalletTransactionQueryRow>(
                sql,
                new
                {
                    MinDate = request.MinDate,
                    MaxDate = request.MaxDate,
                    MinAmount = request.MinAmount,
                    MaxAmount = request.MaxAmount,
                    UserId = userId,
                    Offset = offset,
                    PageSize = request.PageSize
                })).ToList();

            var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

            var result = new PaginatedResult<WalletTransactionResponse>
            {
                Items = rows.Select(x => new WalletTransactionResponse
                {
                    Amount = x.Amount,
                    createdAt = x.CreatedAt,
                    Description = x.Description,
                    OrderId = x.OrderId
                }).ToList(),

                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            return result;
        }
    }
}