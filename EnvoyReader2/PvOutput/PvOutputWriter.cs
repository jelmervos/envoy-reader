using Microsoft.Extensions.Logging;
using PVOutput.Net;
using PVOutput.Net.Builders;
using PVOutput.Net.Objects;

internal class PvOutputWriter : IOutputWriter
{
    private readonly IPVOutputClient pvOutputClient;
    private readonly ILogger logger;
    private readonly DateTimeOffset notBefore = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public PvOutputWriter(ILogger<PvOutputWriter> logger, IPVOutputClient pvOutputClient)
    {
        this.pvOutputClient = pvOutputClient;
        this.logger = logger;
    }

    public async Task Write(InverterData data, float? netFrequency)
    {
        if (data.TimeStamp < notBefore)
        {
            logger.LogError("TimeStamp invalid: {TimeStamp}", data.TimeStamp);
            return;
        }

        var builder = new StatusPostBuilder<IStatusPost>();

        // Build the status
        var status = builder
            .SetTimeStamp(data.TimeStamp.LocalDateTime)
            .SetGeneration(data.GenerationToday, Convert.ToInt32(data.PowerGeneration))
            .SetExtendedValues(netFrequency != null ? Convert.ToDecimal(netFrequency) : null)
            .SetVoltageIfNotNull(data.Voltage)
            .SetTemperatureIfNotNull(data.Temperature)
            .Build();

        logger.LogInformation("Add status for system Id: {SystemId}", pvOutputClient.OwnedSystemId);

        // Push the status back to PVOutput
        var response = await pvOutputClient.Status.AddStatusAsync(status);

        if (response.IsSuccess)
        {
            logger.LogInformation("AddStatus success: {Message}", response.SuccesMessage);
        }
        else
        {
            logger.LogError("AddStatus error: {Message}", response.Error?.Message);
        }
    }
}
