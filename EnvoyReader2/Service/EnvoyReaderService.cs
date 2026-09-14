using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class EnvoyReaderService : BackgroundService
{
    private readonly ILogger<EnvoyReaderService> logger;
    private readonly IPipeline pipeline;
    private readonly ServiceSettings settings;
    private readonly IClock clock;

    public EnvoyReaderService(ILogger<EnvoyReaderService> logger, IPipeline pipeline, IOptions<ServiceSettings> settings, IClock clock)
    {
        this.logger = logger;
        this.pipeline = pipeline;
        this.settings = settings.Value;
        this.clock = clock;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Healthy();
        var interval = TimeSpan.FromMinutes(Math.Max(settings.PipelineIntervalInMinutes, 2));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await pipeline.Start(stoppingToken);
                await Healthy();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Pipeline exception");
                Unhealthy();
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

    private async Task Healthy()
    {
        var file = settings.HealthCheckFile;
        if (string.IsNullOrWhiteSpace(file))
        {
            return;
        }   

        try
        {
            await File.WriteAllTextAsync(file, clock.Now.ToString("o"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while writing health check file");
        }
    }

    private void Unhealthy()
    {
        var file = settings.HealthCheckFile;
        if (string.IsNullOrWhiteSpace(file))
        {
            return;
        }

        try
        {
            File.Delete(file);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while deleting health check file");
        }
    }
}