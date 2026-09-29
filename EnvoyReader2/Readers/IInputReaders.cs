internal interface IInputReaders
{
    Task<InverterData?> ReadInverterData(CancellationToken cancellationToken);
    Task<float?> ReadNetFrequency(CancellationToken cancellationToken);
}