using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

internal class HomeAssistant : INetFrequencyReader
{
    private readonly ILogger<HomeAssistant> logger;
    private readonly IHomeAssistantApi api;
    private readonly IClock clock;
    private readonly IOptions<HomeAssistantSettings> settings;
    private static readonly TimeSpan expiration = TimeSpan.FromMinutes(5);

    public HomeAssistant(ILogger<HomeAssistant> logger, IHomeAssistantApi api, IClock clock, IOptions<HomeAssistantSettings> settings)
    {
        this.logger = logger;
        this.api = api;
        this.clock = clock;
        this.settings = settings;
    }

    public async Task<float?> Read(CancellationToken cancellationToken = default)
    {
        State state;
        try
        {
            state = await api.GetState(settings.Value.NetFreqEntityId);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Could not read net frequency");
            return null;
        }

        logger.LogInformation("State: {State}", state);

        var now = clock.Now;
        if (Utilities.IsExpired(now, state.LastChanged, expiration) && Utilities.IsExpired(now, state.LastUpdated, expiration))
        {
            logger.LogError("Both LastChanged ({LastChanged}) and LastUpdated ({LastUpdated}) are older than {Expiration} minutes",
                state.LastChanged, state.LastUpdated, expiration.TotalMinutes.ToString("0.##"));
            return null;
        }

        if (!float.TryParse(state.StateValue, CultureInfo.InvariantCulture.NumberFormat, out var value))
        {
            logger.LogError("StateValue is not a valid float: {StateValue}", state.StateValue);
            return null;
        }

        return value;
    }
}
