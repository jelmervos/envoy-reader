internal interface IClock
{
    DateTimeOffset Now { get; }
    DateTimeOffset UtcNow { get; }
}