internal interface IOutputWriter
{
    Task Write(InverterData data, float? netFrequency);
}