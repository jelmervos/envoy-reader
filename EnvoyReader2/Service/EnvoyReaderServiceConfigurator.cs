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
                .AddPvOutputClient()
                .AddHomeAssistantOptions()
                .AddSingleton<ISunriseSunset, SunriseSunset>()
                .AddSingleton<IEnvoyClientFactory, EnvoyClientFactory>()
                .AddSingleton<IHealthcheckWriter, HealthcheckWriter>()
                .AddHomeAssistantApi()
                .AddTransient<IInputReaders, InputReaders>()
                .AddTransient<INetFrequencyReader, HomeAssistant>()
                .AddTransient<IInverterDataReader, EnvoyReader>()
                .AddTransient<IOutputWriter, PvOutputWriter>()
                .AddTransient<IAggregator, Aggregator>()
                .AddHostedService<EnvoyReaderService>();
        });
    }
}
