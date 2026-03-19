using System.Reflection;

internal static class Utilities
{
    public static string GetStartupFolder()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
            Path.GetDirectoryName(Environment.ProcessPath) ?? 
            throw new Exception("Could not find startup path");
    }

    public static string FullPath(string file)
    {
        if (Path.IsPathRooted(file))
        {
            return file;
        }
        return Path.GetFullPath(file, GetStartupFolder());
    }

    public static bool IsExpired(DateTimeOffset now, DateTimeOffset then, TimeSpan expiration)
    {
        var timeSpan = now - then;
        return timeSpan >= expiration;
    }
}
