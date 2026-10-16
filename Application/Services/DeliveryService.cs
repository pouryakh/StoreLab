using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IStoreOrderRepository _orderRepository;

        private readonly ILogger<DeliveryService> _logger;

        public DeliveryService(
            IUnitOfWork unitOfWork,
            IStoreOrderRepository orderRepository,
            ILogger<DeliveryService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<SimpleResult> MarkAsDelivered()
        {
            _logger.LogInformation("Check Delivary started");
            var storeOrders = await _orderRepository.GetShippedOrdersAsync();
            var deliveredCounter = 0;

            foreach (var storeOrder in storeOrders) 
            {
                if (storeOrder.EstimatedDeliveryAt <= DateTime.UtcNow)
                {
                    storeOrder.MarkAsDelivered();
                    deliveredCounter++;
                }
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Total Shipped {ShippedCounter} - Total Deliverd {deliverdCounter}", storeOrders.Count(), deliveredCounter);

            return SimpleResult.Success();
        }
    }
}
