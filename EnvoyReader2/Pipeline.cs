using Microsoft.Extensions.Logging;
using System.Diagnostics;

internal class Pipeline : IPipeline
{
    private readonly ILogger<Pipeline> logger;
    private readonly ISunriseSunset sunriseSunset;
    private readonly IInverterDataReader reader;
    private readonly INetFrequencyReader netFrequencyReader;
    private readonly IEnumerable<IOutputWriter> writers;
    private readonly IClock clock;

    public Pipeline(ILogger<Pipeline> logger, ISunriseSunset sunriseSunset,  IInverterDataReader reader, INetFrequencyReader netFrequencyReader,
        IEnumerable<IOutputWriter> writers, IClock clock)
    {
        this.logger = logger;
        this.sunriseSunset = sunriseSunset;
        this.reader = reader;
        this.netFrequencyReader = netFrequencyReader;
        this.writers = writers;
        this.clock = clock;
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Start at {Now}", clock.Now);
        var stopwatch = Stopwatch.StartNew();

        if (sunriseSunset.Usable && !sunriseSunset.IsThereLightOutside())
        {
            logger.LogInformation("It is dark outside");
            return;
        }

        var netFreqTask = ReadNetFrequency(cancellationToken);
        await ReadInverterData(cancellationToken).ContinueWith(async (antecedent) =>
        {
            var inverterData = antecedent.Result;

            if (inverterData == null)
            {
                logger.LogInformation("No inverter data to write");
                return;
            }

            var netFreq = await netFreqTask;

            await WriteOutput(inverterData.Value, netFreq, cancellationToken);
        }, TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();

        stopwatch.Stop();
        logger.LogInformation("All finished at {Now} in {Elapsed}", clock.Now, stopwatch.Elapsed);
    }

    private async Task WriteOutput(InverterData inverterData, float? netFrequency, CancellationToken cancellationToken)
    {
        logger.LogInformation("Write data to {Count} writer(s): {inverterData}, net freqency: {netFrequency}", writers.Count(), inverterData, netFrequency);
        var stopwatch = Stopwatch.StartNew();
        await Parallel.ForEachAsync(writers, cancellationToken, async (writer, token) => await writer.Write(inverterData, netFrequency));
        stopwatch.Stop();
        logger.LogInformation("Finished writing in {Elapsed}", stopwatch.Elapsed);
    }

    private async Task<float?> ReadNetFrequency(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start reading net frequency");

        var stopwatch = Stopwatch.StartNew();
        var value = await netFrequencyReader.Read(cancellationToken);
        stopwatch.Stop();
        logger.LogInformation("Finished reading net frequency in {Elapsed}", stopwatch.Elapsed);

        return value;
    }

    private async Task<InverterData?> ReadInverterData(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start reading inverter data");

        var stopwatch = Stopwatch.StartNew();
        var data = await reader.Read(cancellationToken);
        stopwatch.Stop();
        logger.LogInformation("Finished reading inverter data in {Elapsed}", stopwatch.Elapsed);

        return data;
    }
}
