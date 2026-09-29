internal interface IAggregator
{
    Task Aggregate(CancellationToken cancellationToken = default);
}