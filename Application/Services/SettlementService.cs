using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Common.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SettlementService : ISettlementService
    {
        private readonly IStoreOrderRepository _storeOrderRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IPaymentService _paymentService;

        private readonly ILogger<SettlementService> _logger;

        public SettlementService(
            IStoreOrderRepository storeOrderRepository,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService,
            ILogger<SettlementService> logger
            )
        {
            _storeOrderRepository = storeOrderRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<SimpleResult> MarkEligibleOrdersAsync()
        {
            _logger.LogInformation("MarkEligibleOrdersAsync started.");
            var markedEligibleCount = 0;

            var storeOrders =
                await _storeOrderRepository.GetPendingOrdersReadyForSettlementAsync();

            foreach (var storeOrder in storeOrders)
            {
                storeOrder.MarkAsEligible();

                _logger.LogInformation(
                    "StoreOrder {StoreOrderId} marked as eligible.",
                    storeOrder.Id);

                markedEligibleCount++;
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{MarkedEligibleCount} StoreOrders marked as eligible.", markedEligibleCount);

            return SimpleResult.Success();
        }

        public async Task<SimpleResult> SettleEligibleOrdersAsync()
        {
            _logger.LogInformation("SettleEligibleOrdersAsync started.");

            var storeOrderCount = 0;

            var storeOrders =
                await _storeOrderRepository.GetEligibleOrdersForSettlementAsync();

            foreach (var storeOrder in storeOrders)
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    await _paymentService.SettleStoreOrderAsync(storeOrder);

                    storeOrder.CompleteSettlement();

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation(
                        "StoreOrder {StoreOrderId} settled successfully.",
                        storeOrder.Id);

                    storeOrderCount++;
                }
                catch (DomainException ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    _logger.LogWarning(
                        ex,
                        "Settlement could not be completed for StoreOrder {StoreOrderId} due to a domain rule.",
                        storeOrder.Id);
                }
                catch (InvalidOperationException ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogWarning(
                        ex,
                        "Settlement could not be completed for StoreOrder {StoreOrderId} due to a InvalidOperation rule.",
                        storeOrder.Id);
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    _logger.LogError(
                        ex,
                        "Unexpected error during settlement for StoreOrder {StoreOrderId}.",
                        storeOrder.Id);
                }
            }

            _logger.LogInformation(
                "{TotalStoreOrderCount} StoreOrders processed for settlement and {storeOrderCount} done seccussFully.",
                storeOrders.Count,storeOrderCount);

            return SimpleResult.Success();
        }
    }
}
