using NEnvoy;

internal interface IEnvoyClientFactory
{
    Task<IEnvoyClient> Create(CancellationToken cancellationToken = default);
}
