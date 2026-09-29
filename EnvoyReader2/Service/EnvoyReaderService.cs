using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class EnvoyReaderService : BackgroundService
{
    private readonly ILogger<EnvoyReaderService> logger;
    private readonly IOptions<ServiceSettings> serviceSettings;
    private readonly IAggregator aggregator;
    private readonly IHealthcheckWriter healthcheckWriter;

    public EnvoyReaderService(ILogger<EnvoyReaderService> logger, IOptions<ServiceSettings> serviceSettings, IAggregator aggregator,
        IHealthcheckWriter healthcheckWriter)
    {
        this.logger = logger;
        this.serviceSettings = serviceSettings;
        this.aggregator = aggregator;
        this.healthcheckWriter = healthcheckWriter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await healthcheckWriter.Healthy();

        var minutes = Math.Max(serviceSettings.Value.AggregatorIntervalInMinutes, 2);
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(minutes));

        try
        {
            await RunAggregatorSafely(stoppingToken);

            logger.LogInformation("Waiting {AggregatorInterval} minute(s) before next aggregator run", timer.Period.TotalMinutes);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunAggregatorSafely(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Ignore cancellation
        }
    }

    private async Task RunAggregatorSafely(CancellationToken stoppingToken)
    {
        try
        {
            await aggregator.Aggregate(stoppingToken);
            await healthcheckWriter.Healthy();
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Ignore cancellation
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Aggregator exception");
            healthcheckWriter.Unhealthy();
        }
    }
}