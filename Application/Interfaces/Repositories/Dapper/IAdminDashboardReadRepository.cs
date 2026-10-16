using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.Dapper
{
    public interface IAdminDashboardReadRepository
    {
        Task<AdminDashboardOverviewResponse> GetAdminDashboardOverviewAsync();
        Task<AdminDashboardOrdersDetailsResponse> GetAdminDashboardOrederDetailsAsync();

        Task<AdminDashboardFinanceDetailsResponse> GetAdminDashboardFinanceDetailsAsync();
        Task<AdminDashboardStoresDetailsResponse> GetAdminDashboardStoresDetailsAsync();
    }
}
