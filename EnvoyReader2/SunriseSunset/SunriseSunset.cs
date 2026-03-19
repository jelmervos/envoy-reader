using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class SunriseSunset : ISunriseSunset
{
    private readonly ILogger<SunriseSunset> logger;
    private readonly IClock clock;
    private readonly SystemLocationSettings location;

    public SunriseSunset(ILogger<SunriseSunset> logger, IClock clock, IOptions<SystemLocationSettings> location)
    {
        this.logger = logger;
        this.clock = clock;
        this.location = location.Value;
    }

    public bool IsThereLightOutside()
    {
        if (!Usable)
        {
            throw new InvalidOperationException($"{nameof(SunriseSunset)} is unusable");
        }

        var now = clock.Now;
        Sunriset.CivilTwilight(now.Year, now.Month, now.Day, location.Latitude, location.Longitude,
            out double tsunrise, out double tsunset);

        var sunrise = new DateTimeOffset(DateOnly.FromDateTime(now.Date), TimeOnly.FromTimeSpan(TimeSpan.FromHours(tsunrise)), TimeSpan.Zero).ToLocalTime();
        var sunset = new DateTimeOffset(DateOnly.FromDateTime(now.Date), TimeOnly.FromTimeSpan(TimeSpan.FromHours(tsunset)), TimeSpan.Zero).ToLocalTime();

        logger.LogInformation("Lat: {Latitude}, Long: {Longitude}, Now: {Now}, Rise: {Sunrise}, Set: {Sunset}",
            location.Latitude, location.Longitude, now, sunrise, sunset);

        if ((now > sunrise) && (now < sunset))
        {
            return true;
        }

        return false;
    }

    public bool Usable => location.Latitude > 0 && location.Longitude > 0;
}
