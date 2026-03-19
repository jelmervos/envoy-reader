using Microsoft.Extensions.Logging;
using NEnvoy.Models;

internal sealed class EnvoyReader : IInverterDataReader
{
    private readonly ILogger<EnvoyReader> logger;
    private readonly IEnvoyClientFactory envoyClientFactory;
    private readonly IClock clock;
    private static readonly TimeSpan expiration = TimeSpan.FromMinutes(10);

    public EnvoyReader(ILogger<EnvoyReader> logger, IEnvoyClientFactory envoyClientFactory, IClock clock)
    {
        this.logger = logger;
        this.envoyClientFactory = envoyClientFactory;
        this.clock = clock;
    }

    public async Task<InverterData?> Read(CancellationToken cancellationToken = default)
    {
        var client = await envoyClientFactory.Create(cancellationToken);

        var production = await client.GetProductionAsync(cancellationToken).ConfigureAwait(false);
        var prod = production?.Production?.FirstOrDefault() as Inverters;

        if (prod == null)
        {
            logger.LogError("No inverter production data found");
            return null;
        }

        logger.LogInformation("Production: {Production}", prod);

        if (prod.ActiveCount == 0)
        {
            logger.LogError("No active inverters found");
            return null;
        }

        var now = clock.Now;
        if (Utilities.IsExpired(now, prod.ReadingTime, expiration))
        {
            logger.LogError("Production reading time is older then {Expiration} minutes: {ReadingTime}",
                expiration.TotalMinutes.ToString("0.##"), prod.ReadingTime);
            return null;
        }

        var v1production = await client.GetV1ProductionAsync(cancellationToken).ConfigureAwait(false);
        logger.LogInformation("v1production: {ProductionV1}", v1production);

        var inverterData = new InverterData()
        {
            TimeStamp = prod.ReadingTime,
            PowerGeneration = prod.WattsNow,
            LifeTimeGeneration = Convert.ToInt32(v1production.WattHoursLifetime),
            GenerationToday = Convert.ToInt32(v1production.WattHoursToday)
        };

        DeviceStatus? deviceStatus = null;
        try
        {
            deviceStatus = await client.GetDeviceStatusAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not retrieve device status");
            return inverterData;
        }

        var inverters = deviceStatus?.PCU?
           .Where(v => v.Value["devType"].GetValue<int>() == 1)
           .Select(v => new
           {
               SerialNr = v.Key,
               Temp = v.Value["temperature"].GetValue<int>(),
               ReportDate = DateTimeOffset.FromUnixTimeSeconds(v.Value["reportDate"].GetValue<int>()).ToUniversalTime(),
               DcVoltageIn = v.Value["dcVoltageINmV"].GetValue<int>() / 1000m,
               DcCurrentIn = v.Value["dcCurrentINmA"].GetValue<int>() / 1000m,
               AcVoltageIn = v.Value["acVoltageINmV"].GetValue<int>() / 1000m,
               AcPowerIn = v.Value["acPowerINmW"].GetValue<int>() / 1000m,
               Communicating = v.Value["communicating"].GetValue<bool>(),
               Recent = v.Value["recent"].GetValue<bool>(),
               Producing = v.Value["producing"].GetValue<bool>()
           })
           .ToArray();

        if (inverters == null || inverters.Length == 0)
        {
            logger.LogWarning("No inverter device (PCU) status found");
            return inverterData;
        }

        foreach (var inv in inverters)
        {
            logger.LogInformation("{Inverter}", inv.ToString());
        }

        if (!inverters.Any(i => i.Communicating))
        {
            logger.LogWarning("No inverters communicating");
            return inverterData;
        }

        var voltage = inverters.Where(i => i.AcVoltageIn > 0 && i.Communicating).Average(i => i.AcVoltageIn);
        var temperature = Convert.ToDecimal(inverters.Where(i => i.Communicating).Average(i => i.Temp));
        var timeStamp = new DateTimeOffset(Convert.ToInt64(inverters.Where(i => i.ReportDate > DateTimeOffset.MinValue && i.Communicating).Average(i => i.ReportDate.UtcTicks)), TimeSpan.Zero);

        inverterData = inverterData.WithDetailedInverterData(voltage, temperature);

        return inverterData;
    }
}