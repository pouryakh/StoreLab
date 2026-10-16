using Application.DTOs.StoreDto;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Dapper;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<StoreService> _logger;

        private readonly IStoreRepository _storeRepository;

        private readonly IStoreReadRepository _storeReadRepository;

        private readonly ICurrentUserService _currentUserService;

        private readonly IUserRepository _userRepository;

        private readonly IValidator<CreateStoreRequest> _createStoreValidator;

        private readonly IStoreActivationRepository _activationRepository;

        private readonly IValidator<UpdateStoreRequest> _updateStoreValidator;

        public StoreService(
            IUnitOfWork unitOfWork,
            ILogger<StoreService> logger,
            IStoreRepository storeRepository,
            IStoreReadRepository storeReadRepository,
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            IValidator<CreateStoreRequest> createStoreValidator,
            IStoreActivationRepository activationRepository,
            IValidator<UpdateStoreRequest> updateStoreValidator
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _storeRepository = storeRepository;
            _storeReadRepository = storeReadRepository;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _createStoreValidator = createStoreValidator;
            _activationRepository = activationRepository;
            _updateStoreValidator = updateStoreValidator;
        }

        public async Task<Result<StoreResponse>> CreateStore(CreateStoreRequest request)
        {
            _logger.LogInformation(
                "Creating store {StoreName} by User {UserId}.",
                request.Name,
                _currentUserService.UserId);

            var validationResult = await _createStoreValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Invalid store creation request for Store {StoreName} by User {UserId}.",
                    request.Name,
                    _currentUserService.UserId);

                return Result<StoreResponse>.Failure(
                    validationResult.Errors.First().ErrorMessage);
            }

            if (await _storeRepository.IsNameAlreadyExistsAsync(request.Name.ToLower()))
            {
                _logger.LogWarning(
                    "Store name {StoreName} already exists.",
                    request.Name);

                return Result<StoreResponse>.Failure(
                    "A store with this name already exists.");
            }

            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                _logger.LogWarning(
                    "Unauthenticated user attempted to create store {StoreName}.",
                    request.Name);

                return Result<StoreResponse>.Failure(
                    "Please login first.");
            }

            var store = new Store(
                _currentUserService.UserId.Value,
                request.Name,
                request.Description);

            await _storeRepository.AddAsync(store);

            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value);
            
            if (user is null)
            {
                throw new UnauthorizedAccessException("User account was not found.");
            }

            user.ChangeUserRoleToSeller();

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Store {StoreId} with name {StoreName} created successfully by User {UserId}.",
                store.Id,
                store.Name,
                _currentUserService.UserId);

            var storeResponse = new StoreResponse()
            {
                Name = store.Name,
                Description = store.Description,
                Id = store.Id
            };

            return Result<StoreResponse>.Success(storeResponse);
        }

        public async Task<SimpleResult> DeactivateStore(Guid storeId)
        {
            _logger.LogInformation(
                "Deactivating Store {StoreId} by User {UserId}.",
                storeId,
                _currentUserService.UserId);

            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                _logger.LogWarning(
                    "Unauthenticated user attempted to deactivate Store {StoreId}.",
                    storeId);

                return SimpleResult.Failure(
                    "Please login first.");
            }

            var store = await _storeRepository.GetByIdAsync(storeId);

            if (store is null)
            {
                _logger.LogWarning(
                    "Store {StoreId} was not found.",
                    storeId);

                return SimpleResult.Failure(
                    "Store does not exist.");
            }

            if (store.SellerId != _currentUserService.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to deactivate Store {StoreId} without permission.",
                    _currentUserService.UserId,
                    storeId);

                return SimpleResult.Failure(
                    "Access denied.");
            }

            store.MakeStoreDeactive();

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Store {StoreId} was deactivated successfully by User {UserId}.",
                storeId,
                _currentUserService.UserId);

            return SimpleResult.Success();
        }

        public async Task<Result<List<StoreResponse>>> GetMyStores()
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                _logger.LogWarning(
                    "Unauthenticated user attempted to get their stores.");

                return Result<List<StoreResponse>>.Failure(
                    "Please login first.");
            }

            var stores = await _storeRepository.GetUserAllStoresAsync(
                _currentUserService.UserId.Value);

            var response = stores.Select(x => new StoreResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();

            _logger.LogInformation(
                "Retrieved {StoreCount} stores for User {UserId}.",
                response.Count,
                _currentUserService.UserId);

            return Result<List<StoreResponse>>.Success(response);
        }

        public async Task<Result<StoreResponse>> GetStoreById(Guid storeId)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                _logger.LogWarning(
                    "Unauthenticated user attempted to get Store {StoreId}.",
                    storeId);

                return Result<StoreResponse>.Failure(
                    "Please login first.");
            }

            var store = await _storeRepository.GetByIdAsync(storeId);

            if (store is null)
            {
                _logger.LogWarning(
                    "Store {StoreId} was not found.",
                    storeId);

                return Result<StoreResponse>.Failure(
                    "Store does not exist.");
            }

            if (store.SellerId != _currentUserService.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access Store {StoreId} without permission.",
                    _currentUserService.UserId,
                    store.Id);

                return Result<StoreResponse>.Failure(
                    "Access denied.");
            }

            var storeResponse = new StoreResponse
            {
                Id = store.Id,
                Description = store.Description,
                Name = store.Name
            };

            return Result<StoreResponse>.Success(storeResponse);
        }

        public async Task<SimpleResult> SendActivationRequest(Guid storeId)
        {
            _logger.LogInformation(
                "Starting activation request for Store {StoreId} by User {UserId}.",
                storeId,
                _currentUserService.UserId);

            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                _logger.LogWarning(
                    "Unauthenticated user attempted to send an activation request for Store {StoreId}.",
                    storeId);

                return SimpleResult.Failure(
                    "Please login first.");
            }

            var store = await _storeRepository.GetByIdAsync(storeId);

            if (store is null)
            {
                _logger.LogWarning(
                    "Store {StoreId} was not found.",
                    storeId);

                return SimpleResult.Failure(
                    "Store does not exist.");
            }

            if (!await _storeRepository.IsOwnerOrNot(
                _currentUserService.UserId.Value,
                store.Id))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to send an activation request for Store {StoreId} without permission.",
                    _currentUserService.UserId,
                    storeId);

                return SimpleResult.Failure(
                    "Access denied.");
            }

            if (!store.ActivationRequestCanBeSend())
            {
                _logger.LogWarning(
                    "Store {StoreId} cannot send an activation request because it does not have at least 5 products.",
                    storeId);

                return SimpleResult.Failure(
                    "Please add at least 5 products before requesting activation.");
            }

            var activationrequest = new StoreActivationRequest(storeId);

            store.SubmitForApproval();

            await _activationRepository.AddAsync(activationrequest);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Activation request for Store {StoreId} was submitted successfully by User {UserId}.",
                storeId,
                _currentUserService.UserId);

            return SimpleResult.Success();
        }

        public async Task<Result<StoreResponse>> UpdateStore(UpdateStoreRequest request, Guid storeId)
        {
            _logger.LogInformation(
                "Updating Store {StoreId} by User {UserId}.",
                storeId,
                _currentUserService.UserId);

            var validationResult = await _updateStoreValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Invalid store update request for Store {StoreId} by User {UserId}.",
                    storeId,
                    _currentUserService.UserId);

                return Result<StoreResponse>.Failure(
                    validationResult.Errors.First().ErrorMessage);
            }

            var store = await _storeRepository.GetByIdAsync(storeId);

            if (store is null)
            {
                _logger.LogWarning(
                    "Store {StoreId} was not found.",
                    storeId);

                return Result<StoreResponse>.Failure(
                    "Store does not exist.");
            }

            if (store.SellerId != _currentUserService.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access Store {StoreId} without permission.",
                    _currentUserService.UserId,
                    store.Id);

                return Result<StoreResponse>.Failure(
                    "Access denied.");
            }

            if (!string.IsNullOrWhiteSpace(request.Name) && await _storeRepository.IsNameAlreadyExistsAsync(request.Name.ToLower()))
            {
                _logger.LogWarning(
                    "Store name {StoreName} already exists.",
                    request.Name);

                return Result<StoreResponse>.Failure(
                    "A store with this name already exists.");
            }

            if (request.Description == store.Description)
            {
                _logger.LogWarning(
                    "Store {StoreId} update rejected because the description is unchanged.",
                    storeId);

                return Result<StoreResponse>.Failure(
                    "The description is already the same.");
            }

            store.UpdateDetails(request.Name, request.Description);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Store {StoreId} was updated successfully by User {UserId}.",
                storeId,
                _currentUserService.UserId);

            var storeResponse = new StoreResponse()
            {
                Id = storeId,
                Description = store.Description,
                Name = store.Name,
            };

            return Result<StoreResponse>.Success(storeResponse);
        }

        
    }
}
