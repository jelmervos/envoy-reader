using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class HealthcheckService : BackgroundService
{
    private readonly IHostApplicationLifetime lifetime;
    private readonly ILogger<HealthcheckService> logger;
    private readonly IOptions<HealthcheckSettings> settings;
    private readonly IClock clock;

    public HealthcheckService(IHostApplicationLifetime lifetime, ILogger<HealthcheckService> logger, IOptions<HealthcheckSettings> settings,
        IClock clock)
    {
        this.lifetime = lifetime;
        this.logger = logger;
        this.settings = settings;
        this.clock = clock;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var isSuccess = await DoWorkAsync(stoppingToken);
            logger.LogInformation("Health check result: {IsSuccess}", isSuccess);
            Environment.ExitCode = isSuccess ? 0 : 1;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Health check");
            Environment.ExitCode = 1;
        }
        finally
        {
            lifetime.StopApplication();
        }
    }

    private async Task<bool> DoWorkAsync(CancellationToken cancellationToken = default)
    {
        var healthCheckFile = settings.Value.HealthCheckFile;
        if (string.IsNullOrWhiteSpace(healthCheckFile))
        {
            logger.LogInformation("No check needed: health check file is not configured");
            return true;
        }

        logger.LogInformation("Checking health check file: {HealthCheckFile}", healthCheckFile);
        var fileInfo = new FileInfo(healthCheckFile);

        if (!fileInfo.Exists)
        {
            logger.LogInformation("Unhealthy: health check file does not exist");
            return false;
        }

        var threshold = clock.UtcNow.AddMinutes(-6);

        if (fileInfo.LastWriteTimeUtc < threshold)
        {
            logger.LogInformation("Unhealthy: health check file is outdated: {LastWriteTimeUtc}", fileInfo.LastWriteTimeUtc);
            return false;
        }

        logger.LogInformation("Healthy!");
        return true;
    }
}