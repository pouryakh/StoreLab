using Application.Common.Extensions;
using Application.DTOs.SellerDashboardDto;
using Application.DTOs.SellerDashboardDto.Details;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Dapper;
using Application.Interfaces.Security;
using Application.Interfaces.Services.Dashboards;
using Application.Results;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Dashboards
{
    public class SellerDashboardService : ISellerDashboardService
    {
        private readonly ILogger<SellerDashboardService> _logger;

        private readonly ISellerDashboardReadRepository _sellerDashboardReadRepository;

        private readonly IStoreRepository _storeRepository;

        private readonly ICurrentUserService _currentUserService;

        public SellerDashboardService(
            ILogger<SellerDashboardService> logger,
            ISellerDashboardReadRepository sellerDashboardReadRepository,
            IStoreRepository storeRepository,
            ICurrentUserService currentUserService
            )
        {
            _logger = logger;
            _sellerDashboardReadRepository = sellerDashboardReadRepository;
            _storeRepository = storeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<SellerDashboardOrdersDetailsResponse>> GetOrdersDetails(SellerDashboardOrdersRequest request)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting orders details for Store {StoreId} by User {UserId}.",
                request.StoreId,
                userId);

            if (!await _storeRepository.IsOwnerOrNot(userId, request.StoreId))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access orders details for Store {StoreId} without ownership.",
                    userId,
                    request.StoreId);

                return Result<SellerDashboardOrdersDetailsResponse>.Failure(
                    "Access denied.");
            }

            var result =
                await _sellerDashboardReadRepository.GetOrdersDetails(request);

            _logger.LogInformation(
                "Orders details for Store {StoreId} retrieved successfully by User {UserId}.",
                request.StoreId,
                userId);

            return Result<SellerDashboardOrdersDetailsResponse>.Success(result);
        }

        public async Task<Result<SellerDashboardOverviewResponse>> GetOverview(Guid storeId)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting dashboard overview for Store {StoreId} by User {UserId}.",
                storeId,
                userId);

            if (!await _storeRepository.IsOwnerOrNot(userId, storeId))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access dashboard for Store {StoreId} without ownership.",
                    userId,
                    storeId);

                return Result<SellerDashboardOverviewResponse>.Failure(
                    "Access denied.");
            }

            var result =
                await _sellerDashboardReadRepository.GetOverview(storeId);

            _logger.LogInformation(
                "Dashboard overview for Store {StoreId} retrieved successfully by User {UserId}.",
                storeId,
                userId);

            return Result<SellerDashboardOverviewResponse>.Success(result);
        }

        public async Task<Result<SellerDashboardProductsDetailsResponse>> GetProductDetails(Guid storeId)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting product details for Store {StoreId} by User {UserId}.",
                storeId,
                userId);

            if (!await _storeRepository.IsOwnerOrNot(userId, storeId))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access product details for Store {StoreId} without ownership.",
                    userId,
                    storeId);

                return Result<SellerDashboardProductsDetailsResponse>.Failure(
                    "Access denied.");
            }

            var response =
                await _sellerDashboardReadRepository.GetProductDetails(storeId);

            _logger.LogInformation(
                "Product details for Store {StoreId} retrieved successfully by User {UserId}.",
                storeId,
                userId);

            return Result<SellerDashboardProductsDetailsResponse>.Success(response);
        }

        public async Task<Result<SellerDashboardSettlementDetailsResponse>> GetSettlementDetails(Guid storeId)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting settlement details for Store {StoreId} by User {UserId}.",
                storeId,
                userId);

            if (!await _storeRepository.IsOwnerOrNot(userId, storeId))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access settlement details for Store {StoreId} without ownership.",
                    userId,
                    storeId);

                return Result<SellerDashboardSettlementDetailsResponse>.Failure(
                    "Access denied.");
            }

            var response =
                await _sellerDashboardReadRepository.GetSettlementDetails(storeId);

            _logger.LogInformation(
                "Settlement details for Store {StoreId} retrieved successfully by User {UserId}.",
                storeId,
                userId);

            return Result<SellerDashboardSettlementDetailsResponse>.Success(response);
        }

        public async Task<Result<SellerDashboardTurnoverDetailsResponse>> GetTurnoverDetails(SellerDashboardTurnoverRequest request)
        {
            var userId =
                CurrentUserExtensions.GetRequiredUserId(_currentUserService);

            _logger.LogInformation(
                "Getting turnover details for Store {StoreId} by User {UserId}.",
                request.StoreId,
                userId);

            if (!await _storeRepository.IsOwnerOrNot(userId, request.StoreId))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access turnover details for Store {StoreId} without ownership.",
                    userId,
                    request.StoreId);

                return Result<SellerDashboardTurnoverDetailsResponse>.Failure(
                    "Access denied.");
            }

            var response =
                await _sellerDashboardReadRepository.GetTurnoverDetails(request);

            _logger.LogInformation(
                "Turnover details for Store {StoreId} retrieved successfully by User {UserId}.",
                request.StoreId,
                userId);

            return Result<SellerDashboardTurnoverDetailsResponse>.Success(response);
        }
    }
}
