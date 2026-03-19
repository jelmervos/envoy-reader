internal class Clock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
