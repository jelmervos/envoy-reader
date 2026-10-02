using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

internal class PvOutputWriter : IOutputWriter
{
    private const string PvOutputAddStatusUrl = "https://pvoutput.org/service/r2/addstatus.jsp";
    private readonly IOptions<PvOutputSettings> settings;
    private readonly ILogger logger;
    private readonly DateTimeOffset notBefore = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public PvOutputWriter(ILogger<PvOutputWriter> logger, IOptions<PvOutputSettings> settings)
    {
        this.settings = settings;
        this.logger = logger;
    }

    public async Task Write(InverterData data, float? netFrequency)
    {
        if (data.TimeStamp < notBefore)
        {
            logger.LogError("TimeStamp invalid: {TimeStamp}", data.TimeStamp);
            return;
        }

        var systemId = settings.Value.SystemId.ToString();

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Pvoutput-Apikey", settings.Value.ApiKey);
        client.DefaultRequestHeaders.Add("X-Pvoutput-SystemId", systemId);

        var timestamp = data.TimeStamp.LocalDateTime;

        var postData = new Dictionary<string, string>
        {
            ["d"] = timestamp.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
            ["t"] = timestamp.ToString("HH:mm", CultureInfo.InvariantCulture),
            ["v1"] = data.GenerationToday.ToString(), // Energy Generation Today (Wh)
            ["v2"] = Convert.ToInt32(data.PowerGeneration).ToString(), // Power Generation (W)
        };

        if (data.Temperature != null)
        {
            postData["v5"] = data.Temperature.Value.ToString("F1", CultureInfo.InvariantCulture); // Temperature (°C)
        }

        if (data.Voltage != null)
        {
            postData["v6"] = data.Voltage.Value.ToString("F1", CultureInfo.InvariantCulture); // Voltage (V)
        }

        if (netFrequency != null)
        {
            postData["v7"] = netFrequency.Value.ToString("F1", CultureInfo.InvariantCulture); // Extended Value 1
        }

        logger.LogInformation("Add status for system Id: {SystemId}, date: {Date}, time: {Time}",
            systemId, postData["d"], postData["t"]);
        
        var content = new FormUrlEncodedContent(postData);
        var response = await client.PostAsync(PvOutputAddStatusUrl, content);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("addstatus success: {ReasonPhrase}", response.ReasonPhrase);
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("addstatus error ({StatusCode}): {Message}", response.StatusCode, error);
        }
    }
}
