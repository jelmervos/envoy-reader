internal interface IPipeline
{
    Task Start(CancellationToken cancellationToken = default);
}