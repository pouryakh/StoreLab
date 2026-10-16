using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Dashboards
{
    public interface IAdminDashboardService
    {
        Task<Result<AdminDashboardOverviewResponse>> GetAdminDashboardOverview();
        Task<Result<AdminDashboardOrdersDetailsResponse>> GetAdminDashboardOrderDetails();
        Task<Result<AdminDashboardFinanceDetailsResponse>> GetAdminDashboardFinanceDetails();
        Task<Result<AdminDashboardStoresDetailsResponse>> GetAdminDashboardStoresDetails();

        
    }
}
