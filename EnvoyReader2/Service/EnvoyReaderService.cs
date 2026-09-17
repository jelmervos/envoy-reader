using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class EnvoyReaderService : BackgroundService
{
    private readonly ILogger<EnvoyReaderService> logger;
    private readonly IPipeline pipeline;
    private readonly IOptions<ServiceSettings> serviceSettings;
    private readonly IHealthcheckWriter healthcheckWriter;

    public EnvoyReaderService(ILogger<EnvoyReaderService> logger, IPipeline pipeline, IOptions<ServiceSettings> serviceSettings,
        IHealthcheckWriter healthcheckWriter)
    {
        this.logger = logger;
        this.pipeline = pipeline;
        this.serviceSettings = serviceSettings;
        this.healthcheckWriter = healthcheckWriter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await healthcheckWriter.Healthy();
        var interval = TimeSpan.FromMinutes(Math.Max(serviceSettings.Value.PipelineIntervalInMinutes, 2));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await pipeline.Start(stoppingToken);
                await healthcheckWriter.Healthy();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Pipeline exception");
                healthcheckWriter.Unhealthy();
            }

            try
            {
                logger.LogInformation("Waiting {PipelineInterval} minute(s) before next pipeline run", Convert.ToInt32(interval.TotalMinutes));
                await Task.Delay(interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}