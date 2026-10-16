using Application.DTOs.SellerDashboardDto;
using Application.DTOs.SellerDashboardDto.Details;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Dashboards
{
    public interface ISellerDashboardService
    {
        Task<Result<SellerDashboardOverviewResponse>> GetOverview(Guid storeId);
        Task<Result<SellerDashboardOrdersDetailsResponse>> GetOrdersDetails(SellerDashboardOrdersRequest request);
        Task<Result<SellerDashboardTurnoverDetailsResponse>> GetTurnoverDetails(SellerDashboardTurnoverRequest request);
        Task<Result<SellerDashboardSettlementDetailsResponse>> GetSettlementDetails(Guid storeId);
        Task<Result<SellerDashboardProductsDetailsResponse>> GetProductDetails(Guid storeId);
    }
}
