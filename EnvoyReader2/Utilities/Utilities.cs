internal static class Utilities
{
    public static string GetStartupFolder() =>
        Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;

    public static string FullPath(string file) =>
        Path.GetFullPath(file, GetStartupFolder());

    public static bool IsExpired(DateTimeOffset now, DateTimeOffset then, TimeSpan expiration)
    {
        var timeSpan = now - then;
        return timeSpan >= expiration;
    }
}
