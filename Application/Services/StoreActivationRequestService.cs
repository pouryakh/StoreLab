using Application.DTOs.ActivationDto;
using Application.DTOs.CommonDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Dapper;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Results;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class StoreActivationRequestService : IStoreActivationRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ICurrentUserService _currentUserService;

        private readonly ILogger<StoreActivationRequestService> _logger;

        private readonly IActivationRequestReadRepository _activationReadRepository;

        private readonly IStoreActivationRepository _activationRepository;

        private readonly IValidator<PaginationRequest> _paginationRequestValidator;

        public StoreActivationRequestService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<StoreActivationRequestService> logger,
            IActivationRequestReadRepository activationReadRepository,
            IStoreActivationRepository activationRepository,
            IValidator<PaginationRequest> paginationRequestValidator
            )
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
            _activationReadRepository = activationReadRepository;
            _activationRepository = activationRepository;
            _paginationRequestValidator = paginationRequestValidator;
        }

        public async Task<SimpleResult> ApproveActivationRequest(
            Guid activationRequestId,
            string? note)
        {
            _logger.LogInformation(
                "Approving activation request {ActivationRequestId} by User {UserId}.",
                activationRequestId,
                _currentUserService.UserId);

            if (!_currentUserService.IsAuthenticated ||
                _currentUserService.UserId is null)
            {
                _logger.LogWarning(
                    "Unauthenticated user attempted to approve activation request {ActivationRequestId}.",
                    activationRequestId);

                return SimpleResult.Failure(
                    "Please login first.");
            }

            var activationRequest =
                await _activationRepository.GetByIdAsync(activationRequestId);

            if (activationRequest is null)
            {
                _logger.LogWarning(
                    "Activation request {ActivationRequestId} was not found.",
                    activationRequestId);

                return SimpleResult.Failure(
                    "Activation request does not exist.");
            }

            activationRequest.Store.Approve();

            activationRequest.Approve(note);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Activation request {ActivationRequestId} for Store {StoreId} was approved by User {UserId}.",
                activationRequestId,
                activationRequest.StoreId,
                _currentUserService.UserId);

            return SimpleResult.Success();
        }

        public async Task<Result<PaginatedResult<ActivationResponse>>>
    GetPendingActivationRequests(PaginationRequest request)
        {
            var validationResult =
                await _paginationRequestValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Invalid pagination request for pending activation requests by User {UserId}.",
                    _currentUserService.UserId);

                return Result<PaginatedResult<ActivationResponse>>.Failure(
                    validationResult.Errors.First().ErrorMessage);
            }

            if (!_currentUserService.IsAuthenticated ||
                _currentUserService.UserId is null)
            {
                _logger.LogWarning(
                    "Unauthenticated user attempted to get pending activation requests.");

                return Result<PaginatedResult<ActivationResponse>>.Failure(
                    "Please login first.");
            }

            var rows =
                await _activationReadRepository
                    .GetPendingActivationRequestsAsync(request);

            var totalCount =
                rows.FirstOrDefault()?.TotalCount ?? 0;

            var items = rows
                .GroupBy(x => new
                {
                    x.ActivationRequestId,
                    x.StoreId,
                    x.StoreName,
                    x.StoreDescription
                })
                .Select(g => new ActivationResponse
                {
                    ActivationRequestId = g.Key.ActivationRequestId,
                    StoreId = g.Key.StoreId,
                    StoreName = g.Key.StoreName,
                    StoreDescription = g.Key.StoreDescription,

                    Products = g
                        .Select(x => new ActivationProductResponse
                        {
                            Id = x.ProductId,
                            Name = x.ProductName,
                            Price = x.ProductPrice
                        })
                        .ToList()
                })
                .ToList();

            var result = new PaginatedResult<ActivationResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            return Result<PaginatedResult<ActivationResponse>>
                .Success(result);
        }

        public async Task<SimpleResult> RejectActivationRequest(
            Guid activationRequestId,
            string? note)
        {
            _logger.LogInformation(
                "Rejecting activation request {ActivationRequestId} by User {UserId}.",
                activationRequestId,
                _currentUserService.UserId);

            if (!_currentUserService.IsAuthenticated ||
                _currentUserService.UserId is null)
            {
                _logger.LogWarning(
                    "Unauthenticated user attempted to reject activation request {ActivationRequestId}.",
                    activationRequestId);

                return SimpleResult.Failure(
                    "Please login first.");
            }

            var activationRequest =
                await _activationRepository.GetByIdAsync(activationRequestId);

            if (activationRequest is null)
            {
                _logger.LogWarning(
                    "Activation request {ActivationRequestId} was not found.",
                    activationRequestId);

                return SimpleResult.Failure(
                    "Activation request does not exist.");
            }

            activationRequest.Store.Reject();

            activationRequest.Reject(note);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Activation request {ActivationRequestId} for Store {StoreId} was rejected by User {UserId}.",
                activationRequestId,
                activationRequest.StoreId,
                _currentUserService.UserId);

            return SimpleResult.Success();
        }
    }
}
