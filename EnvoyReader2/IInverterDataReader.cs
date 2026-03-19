internal interface IInverterDataReader
{
    Task<InverterData?> Read(CancellationToken cancellationToken = default);
}