using Microsoft.Extensions.Logging;
using System.Diagnostics;

internal class InputReaders : IInputReaders
{
    private readonly IInverterDataReader inverterReader;
    private readonly INetFrequencyReader netFrequencyReader;
    private readonly ILogger<InputReaders> logger;

    public InputReaders(ILogger<InputReaders> logger, IInverterDataReader inverterReader, INetFrequencyReader netFrequencyReader)
    {
        this.logger = logger;
        this.inverterReader = inverterReader;
        this.netFrequencyReader = netFrequencyReader;
    }

    public async Task<InverterData?> ReadInverterData(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start reading inverter data");

        var stopwatch = Stopwatch.StartNew();
        var data = await inverterReader.Read(cancellationToken);
        stopwatch.Stop();
        logger.LogInformation("Finished reading inverter data in {Elapsed}", stopwatch.Elapsed);

        return data;
    }


    public async Task<float?> ReadNetFrequency(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start reading net frequency");

        var stopwatch = Stopwatch.StartNew();
        var value = await netFrequencyReader.Read(cancellationToken);
        stopwatch.Stop();
        logger.LogInformation("Finished reading net frequency in {Elapsed}", stopwatch.Elapsed);

        return value;
    }
}
