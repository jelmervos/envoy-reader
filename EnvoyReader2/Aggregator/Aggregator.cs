using Microsoft.Extensions.Logging;
using System.Diagnostics;

internal class Aggregator : IAggregator
{
    private readonly ILogger<Aggregator> logger;
    private readonly ISunriseSunset sunriseSunset;
    private readonly IInputReaders readers;
    private readonly IReadOnlyCollection<IOutputWriter> writers;
    private readonly IClock clock;

    public Aggregator(ILogger<Aggregator> logger, ISunriseSunset sunriseSunset, IInputReaders readers, IEnumerable<IOutputWriter> writers, IClock clock)
    {
        this.logger = logger;
        this.sunriseSunset = sunriseSunset;
        this.readers = readers;
        this.writers = writers as IReadOnlyCollection<IOutputWriter> ?? writers.ToArray();
        this.clock = clock;
    }

    public async Task Aggregate(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Start at {Now}", clock.Now);
        var stopwatch = Stopwatch.StartNew();

        if (sunriseSunset.Usable && !sunriseSunset.IsThereLightOutside())
        {
            logger.LogInformation("It is dark outside");
            return;
        }


        var netFreqTask = readers.ReadNetFrequency(cancellationToken);
        var inverterData = await readers.ReadInverterData(cancellationToken);

        if (inverterData == null)
        {
            logger.LogInformation("No inverter data to write");
            return;
        }

        var netFreq = await netFreqTask;
        await WriteOutput(inverterData.Value, netFreq, cancellationToken);

        stopwatch.Stop();
        logger.LogInformation("All finished at {Now} in {Elapsed}", clock.Now, stopwatch.Elapsed);
    }

    private async Task WriteOutput(InverterData inverterData, float? netFrequency, CancellationToken cancellationToken)
    {
        logger.LogInformation("Write data to {Count} writer(s): {inverterData}, net freqency: {netFrequency}", writers.Count, inverterData, netFrequency);
        var stopwatch = Stopwatch.StartNew();
        await Parallel.ForEachAsync(writers, cancellationToken, async (writer, token) => await writer.Write(inverterData, netFrequency));
        stopwatch.Stop();
        logger.LogInformation("Finished writing in {Elapsed}", stopwatch.Elapsed);
    }
}
