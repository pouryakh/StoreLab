using Application.DTOs.ActivationDto;
using Application.DTOs.CommonDto;
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
    public class ActivationRequestReadRepository : IActivationRequestReadRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ActivationRequestReadRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ActivationRequestQueryRow>> GetPendingActivationRequestsAsync(PaginationRequest request)
        {
            var offset = (request.PageNumber - 1) * request.PageSize;

            using var connection = _connectionFactory.CreateConnection();

            const string sql =
                """
                WITH PagedRequests AS
                (
                    SELECT
                        sr.Id AS ActivationRequestId,
                        s.Id AS StoreId,
                        s.Name AS StoreName,
                        s.Description AS StoreDescription,

                        COUNT(*) OVER() AS TotalCount,

                        ROW_NUMBER() OVER (
                            ORDER BY sr.CreatedAt ASC
                        ) AS RequestRowNumber

                    FROM StoreActivationRequests sr

                    INNER JOIN Store s
                        ON sr.StoreId = s.Id

                    WHERE sr.Status = 1
                ),

                ProductsWithRowNumber AS
                (
                    SELECT
                        sr.Id AS ActivationRequestId,
                        p.Id AS ProductId,
                        p.Name AS ProductName,
                        p.Price AS ProductPrice,

                        ROW_NUMBER() OVER (
                            PARTITION BY p.StoreId
                            ORDER BY p.CreatedAt DESC
                        ) AS ProductRowNumber

                    FROM StoreActivationRequests sr

                    INNER JOIN Products p
                        ON p.StoreId = sr.StoreId

                    WHERE sr.Status = 1
                )
                SELECT
                    pr.ActivationRequestId,
                    pr.StoreId,
                    pr.StoreName,
                    pr.StoreDescription,
                    pr.TotalCount,
                    p.ProductId,
                    p.ProductName,
                    p.ProductPrice

                FROM PagedRequests pr

                INNER JOIN ProductsWithRowNumber p
                    ON pr.ActivationRequestId = p.ActivationRequestId

                WHERE pr.RequestRowNumber > @RequestOffset
                  AND pr.RequestRowNumber <= @MaxNumber

                  AND p.ProductRowNumber <= 3

                ORDER BY
                    pr.RequestRowNumber,
                    p.ProductRowNumber;
                """;

            return await connection.QueryAsync<ActivationRequestQueryRow>(sql, 
                new
                {
                    RequestOffset = offset,
                    MaxNumber = request.PageSize + offset,
                });
        }
    }
}
