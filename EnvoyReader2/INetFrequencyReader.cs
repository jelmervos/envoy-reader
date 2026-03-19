internal interface INetFrequencyReader
{
    Task<float?> Read(CancellationToken cancellationToken = default);
}

