internal static class HealthCheckHelper
{
    internal static bool TryGetHealthCheckFileFromArgs(string[] args, out string? healthCheckFile)
    {
        healthCheckFile = null;
        int index = Array.IndexOf(args, "--healthcheck");

        if (index == -1)
            return false;

        healthCheckFile = (index + 1 < args.Length)
            ? args[index + 1]
            : null;

        return true;
    }

    internal static int RunHealthCheck(string healthCheckFile)
    {
        var fileInfo = new FileInfo(healthCheckFile);

        if (!fileInfo.Exists)
        {
            return 1;
        }

        var threshold = DateTimeOffset.UtcNow.AddMinutes(-6);

        if (fileInfo.LastWriteTimeUtc >= threshold)
        {
            return 0;
        }

        return 1;
    }
}