using Application.DTOs.ProductDto;
using Application.Interfaces.Repositories.Dapper;
using Application.Results;
using Dapper;
using Infrastructure.Persistence.Connection;

namespace Infrastructure.Repository.Dapper
{
    public class ProductReadRepository : IProductReadRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductReadRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _connectionFactory = dbConnectionFactory;
        }

        public async Task<PaginatedResult<ProductResponse>> GetProductsAsync(
            ProductQueryRequest request)
        {
            if (request.Page < 1)
                request.Page = 1;

            if (request.PageSize < 1)
                request.PageSize = 10;

            var offset = (request.Page - 1) * request.PageSize;

            const string sql =
                """
                SELECT COUNT(*)
                FROM Products AS p
                WHERE
                    p.IsActive = 1
                    AND p.StockQuantity > 0

                    AND (
                        @Search IS NULL
                        OR p.Name LIKE '%' + @Search + '%'
                        OR p.Description LIKE '%' + @Search + '%'
                    )

                    AND (
                        @StoreId IS NULL
                        OR p.StoreId = @StoreId
                    )

                    AND (
                        @MinPrice IS NULL
                        OR p.Price >= @MinPrice
                    )

                    AND (
                        @MaxPrice IS NULL
                        OR p.Price <= @MaxPrice
                    );


                SELECT
                    p.Id,
                    p.Name,
                    p.StoreId,
                    s.Name AS StoreName,
                    p.Description,
                    p.Price,
                    p.StockQuantity,
                    p.IsActive
                FROM Products AS p
                INNER JOIN Store AS s
                    ON p.StoreId = s.Id
                WHERE
                    p.IsActive = 1
                    AND p.StockQuantity > 0

                    AND (
                        @Search IS NULL
                        OR p.Name LIKE '%' + @Search + '%'
                        OR p.Description LIKE '%' + @Search + '%'
                    )

                    AND (
                        @StoreId IS NULL
                        OR p.StoreId = @StoreId
                    )

                    AND (
                        @MinPrice IS NULL
                        OR p.Price >= @MinPrice
                    )

                    AND (
                        @MaxPrice IS NULL
                        OR p.Price <= @MaxPrice
                    )

                ORDER BY p.CreatedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;
                """;

            using var connection = _connectionFactory.CreateConnection();

            using var result = await connection.QueryMultipleAsync(
                sql,
                new
                {
                    Search = string.IsNullOrWhiteSpace(request.Search)
                        ? null
                        : request.Search.Trim(),

                    StoreId = request.StoreId,

                    MinPrice = request.MinPrice,

                    MaxPrice = request.MaxPrice,

                    Offset = offset,

                    PageSize = request.PageSize
                });

            var totalCount = await result.ReadSingleAsync<int>();

            var items = (await result.ReadAsync<ProductResponse>()).ToList();

            return new PaginatedResult<ProductResponse>
            {
                Items = items,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}