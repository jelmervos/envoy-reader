using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class HealthcheckWriter : IHealthcheckWriter
{
    private readonly IOptions<HealthcheckSettings> settings;
    private readonly ILogger logger;
    private readonly IClock clock;

    public HealthcheckWriter(IOptions<HealthcheckSettings> settings, ILogger<HealthcheckWriter> logger, IClock clock)
    {
        this.settings = settings;
        this.logger = logger;
        this.clock = clock;
    }

    public async Task Healthy()
    {
        var file = settings.Value.HealthCheckFile;
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

    public void Unhealthy()
    {
        logger.LogWarning("Unhealthy!");

        var file = settings.Value.HealthCheckFile;
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
