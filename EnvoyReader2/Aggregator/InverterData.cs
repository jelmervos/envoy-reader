internal readonly record struct InverterData
{
    public DateTimeOffset TimeStamp { get; init; }
    public int LifeTimeGeneration { get; init; }
    public int GenerationToday { get; init; }
    public decimal PowerGeneration { get; init; }
    public decimal? Voltage { get; init; }
    public decimal? Temperature { get; init; }
}

internal static class InverterDataExtensions
{
    public static InverterData WithDetailedInverterData(this InverterData inverterData, decimal voltage, decimal temperature)
    {
        return new InverterData()
        {
            TimeStamp = inverterData.TimeStamp,
            LifeTimeGeneration = inverterData.LifeTimeGeneration,
            GenerationToday = inverterData.GenerationToday,
            PowerGeneration = inverterData.PowerGeneration,
            Voltage = voltage,
            Temperature = temperature
        };
    }

    public static InverterData WithDetailedInverterData(this InverterData inverterData, decimal voltage, decimal temperature, DateTimeOffset timeStamp)
    {
        return new InverterData()
        {
            TimeStamp = timeStamp,
            LifeTimeGeneration = inverterData.LifeTimeGeneration,
            GenerationToday = inverterData.GenerationToday,
            PowerGeneration = inverterData.PowerGeneration,
            Voltage = voltage,
            Temperature = temperature
        };
    }
}
