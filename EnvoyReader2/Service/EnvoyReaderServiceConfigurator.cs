using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal static class EnvoyReaderServiceConfigurator
{
    public static void ConfigureServices(IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services
                .AddEnvoyOptions()
                .AddServiceOptions()
                .AddHealthcheckOptions()
                .AddSunriseSunsetOptions()
                .AddPvOutputOptions()
                .AddHomeAssistantOptions()
                .AddSingleton<ISunriseSunset, SunriseSunset>()
                .AddSingleton<IEnvoyClientFactory, EnvoyClientFactory>()
                .AddSingleton<IHealthcheckWriter, HealthcheckWriter>()
                .AddPvOutputClient()
                .AddHomeAssistantApi()
                .AddTransient<INetFrequencyReader, HomeAssistant>()
                .AddTransient<IInverterDataReader, EnvoyReader>()
                .AddTransient<IOutputWriter, PvOutputWriter>()
                .AddTransient<IPipeline, Pipeline>()
                .AddHostedService<EnvoyReaderService>();
        });
    }
}
