using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class EnvoyReaderService : BackgroundService
{
    private readonly ILogger<EnvoyReaderService> logger;
    private readonly IPipeline pipeline;
    private readonly ServiceSettings settings;

    public EnvoyReaderService(ILogger<EnvoyReaderService> logger, IPipeline pipeline, IOptions<ServiceSettings> settings)
    {
        this.logger = logger;
        this.pipeline = pipeline;
        this.settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(settings.PipelineIntervalInMinutes);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await pipeline.Start(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Pipeline exception");
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