using Application.DTOs.SellerDashboardDto;
using Application.DTOs.SellerDashboardDto.Details;
using Application.Services.Dashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.Dapper
{
    public interface ISellerDashboardReadRepository
    {
        Task<SellerDashboardOverviewResponse> GetOverview(Guid storeId);
        Task<SellerDashboardOrdersDetailsResponse> GetOrdersDetails(SellerDashboardOrdersRequest request);

        Task<SellerDashboardTurnoverDetailsResponse> GetTurnoverDetails(SellerDashboardTurnoverRequest request);

        Task<SellerDashboardSettlementDetailsResponse> GetSettlementDetails(Guid storeId);
        Task<SellerDashboardProductsDetailsResponse> GetProductDetails(Guid storeId);
    }
}
