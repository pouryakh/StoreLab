using Application.Interfaces.Services;

namespace API.BackgroundServices
{
    public class SettlementBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SettlementBackgroundService> _logger;

        public SettlementBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<SettlementBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Settlement Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var settlementService =
                        scope.ServiceProvider
                            .GetRequiredService<ISettlementService>();

                    await settlementService.MarkEligibleOrdersAsync();

                    await settlementService.SettleEligibleOrdersAsync();
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(
                        "Settlement Background Service is stopping.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An error occurred while processing settlement.");
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
                "Settlement Background Service stopped.");
        }
    }
}
