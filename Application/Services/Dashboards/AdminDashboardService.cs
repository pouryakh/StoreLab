using Application.Common.Extensions;
using Application.Interfaces.Repositories.Dapper;
using Application.Interfaces.Security;
using Application.Interfaces.Services.Dashboards;
using Application.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Dashboards
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardReadRepository _adminDashboardReadRepository;

        private readonly ILogger<AdminDashboardService> _logger;

        private readonly ICurrentUserService _currentUserService;

        public AdminDashboardService(
            IAdminDashboardReadRepository adminDashboardReadRepository,
            ILogger<AdminDashboardService> logger,
            ICurrentUserService currentUserService
            )
        {
            _adminDashboardReadRepository = adminDashboardReadRepository;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result<AdminDashboardFinanceDetailsResponse>> GetAdminDashboardFinanceDetails()
        {
            var userId = CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting admin dashboard finance details for UserId: {UserId}",
                userId);

            var response = await _adminDashboardReadRepository.GetAdminDashboardFinanceDetailsAsync();

            _logger.LogInformation(
                "Admin dashboard finance details retrieved successfully for UserId: {UserId}",
                userId);

            return Result<AdminDashboardFinanceDetailsResponse>.Success(response);
        }

        public async Task<Result<AdminDashboardOrdersDetailsResponse>> GetAdminDashboardOrderDetails()
        {
            var userId = CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting admin orders dashboard for UserId: {UserId}",
                userId);

            var response = await _adminDashboardReadRepository.GetAdminDashboardOrederDetailsAsync();

            _logger.LogInformation(
                "Admin orders dashboard retrieved successfully for UserId: {UserId}",
                userId);

            return Result<AdminDashboardOrdersDetailsResponse>.Success(response);
        }

        public async Task<Result<AdminDashboardOverviewResponse>> GetAdminDashboardOverview()
        {
            var userId = CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting admin dashboard overview for UserId: {UserId}",
                userId);

            var response = await _adminDashboardReadRepository.GetAdminDashboardOverviewAsync();

            _logger.LogInformation(
                "Admin dashboard overview retrieved successfully for UserId: {UserId}",
                userId);

            return Result<AdminDashboardOverviewResponse>.Success(response);
        }

        public async Task<Result<AdminDashboardStoresDetailsResponse>> GetAdminDashboardStoresDetails()
        {
            var userId = CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting admin dashboard stores details for UserId: {UserId}",
                userId);

            var response = await _adminDashboardReadRepository.GetAdminDashboardStoresDetailsAsync();

            _logger.LogInformation(
                "Admin dashboard stores details retrieved successfully for UserId: {UserId}",
                userId);

            return Result<AdminDashboardStoresDetailsResponse>.Success(response);
        }
    }
}
