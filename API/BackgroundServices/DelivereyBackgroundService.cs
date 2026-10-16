using Application.Interfaces.Services;

namespace API.BackgroundServices
{
    public class DeliveryBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeliveryBackgroundService> _logger;

        public DeliveryBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DeliveryBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Delivery Background Service Started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var deliveryService =
                        scope.ServiceProvider
                            .GetRequiredService<IDeliveryService>();

                    await deliveryService.MarkAsDelivered();
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(
                        "Delivery Background Service is stopping.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An error occurred while processing Delivery.");
                }

                try
                {
                    await Task.Delay(
                        TimeSpan.FromHours(1),
                        stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.LogInformation(
                "Delivery Background Service stopped.");
        }
    }
}
