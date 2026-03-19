using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NEnvoy;
using NEnvoy.Models;

internal class EnvoyClientFactory : IEnvoyClientFactory
{
    private readonly ILogger<EnvoyClientFactory> logger;
    private readonly IOptions<EnvoyClientSettings> settings;

    public EnvoyClientFactory(ILogger<EnvoyClientFactory> logger, IOptions<EnvoyClientSettings> settings)
    {
        this.logger = logger;
        this.settings = settings;
    }

    public async Task<IEnvoyClient> Create(CancellationToken cancellationToken = default)
    {
        EnvoyClient client;
        var connectionInfo = new EnvoyConnectionInfo()
        {
            EnvoyHost = settings.Value.Host,
            Username = settings.Value.Username,
            Password = settings.Value.Password
        };

        var tokenFile = Utilities.FullPath(settings.Value.TokenFile);

        logger.LogInformation("Create client for {Host}, token file: {TokenFile}", connectionInfo.EnvoyHost, tokenFile);

        if (File.Exists(tokenFile))
        {
            logger.LogInformation("Use token");
            var token = await File.ReadAllTextAsync(tokenFile, cancellationToken).ConfigureAwait(false);
            client = EnvoyClient.FromToken(token, connectionInfo);
            try
            {
                var info = await client.GetHomeAsync(cancellationToken);
                return client;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                logger.LogInformation("Unauthorized, try getting new token");
            }
        }

        logger.LogInformation("Use login");
        client = await EnvoyClient.FromLoginAsync(connectionInfo, cancellationToken).ConfigureAwait(false);
        await File.WriteAllTextAsync(tokenFile, client.GetToken(), cancellationToken).ConfigureAwait(false);
        return client;
    }
}
