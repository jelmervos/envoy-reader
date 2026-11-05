using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

internal class PvOutputWriter : IOutputWriter
{
    private const string PvOutputAddStatusPath = "service/r2/addstatus.jsp";
    private readonly HttpClient httpClient;
    private readonly IOptions<PvOutputSettings> settings;
    private readonly ILogger logger;
    private readonly DateTimeOffset notBefore = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public PvOutputWriter(HttpClient httpClient, ILogger<PvOutputWriter> logger, IOptions<PvOutputSettings> settings)
    {
        this.httpClient = httpClient;
        this.settings = settings;
        this.logger = logger;
    }

    public async Task Write(InverterData data, float? netFrequency, CancellationToken cancellationToken = default)
    {
        if (data.TimeStamp < notBefore)
        {
            logger.LogError("TimeStamp invalid: {TimeStamp}", data.TimeStamp);
            return;
        }

        var systemId = settings.Value.SystemId.ToString();

        var timestamp = data.TimeStamp.LocalDateTime;

        var postData = new Dictionary<string, string>
        {
            ["d"] = timestamp.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
            ["t"] = timestamp.ToString("HH:mm", CultureInfo.InvariantCulture),
            ["v1"] = data.GenerationToday.ToString(CultureInfo.InvariantCulture), // Energy Generation Today (Wh)
            ["v2"] = Convert.ToInt32(data.PowerGeneration).ToString(CultureInfo.InvariantCulture), // Power Generation (W)
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

        using var request = new HttpRequestMessage(HttpMethod.Post, PvOutputAddStatusPath)
        {
            Content = new FormUrlEncodedContent(postData)
        };
        request.Headers.Add("X-Pvoutput-Apikey", settings.Value.ApiKey);
        request.Headers.Add("X-Pvoutput-SystemId", systemId);

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("addstatus success: {ReasonPhrase}", response.ReasonPhrase);
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("addstatus error ({StatusCode}): {Message}", response.StatusCode, error);
        }
    }
}
