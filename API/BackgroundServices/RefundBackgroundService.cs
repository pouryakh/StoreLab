
using Application.Interfaces;
using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices;

public sealed class RefundBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefundBackgroundService> _logger;

    public RefundBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<RefundBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromDays(5));

        _logger.LogInformation(
            "Refund background service started. Interval: 5 days.");

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var refundService = scope.ServiceProvider
                        .GetRequiredService<IRefundService>();

                    _logger.LogInformation(
                        "Refund background job started.");

                    var result =
                        await refundService.RefundEligibleOrdersAsync();

                    _logger.LogInformation(
                        "Refund background job finished. Success: {Success}.",
                        result.IsSuccess);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An unexpected error occurred during refund processing.");
                }
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Refund background service is stopping.");
        }
    }
}
