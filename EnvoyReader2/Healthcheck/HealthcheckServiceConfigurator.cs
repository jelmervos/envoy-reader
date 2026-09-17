using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal static class HealthcheckServiceConfigurator
{
    public static void ConfigureServices(IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services
                .AddHealthcheckOptions()
                .AddHostedService<HealthcheckService>();
        });
    }
}
