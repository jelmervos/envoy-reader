using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host.CreateDefaultBuilder(args)
    .UseContentRoot(Utilities.GetStartupFolder())
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(builder => builder.AddConsole())
            .AddApplicationOptions()
            .AddSingleton<IClock, Clock>()
            .AddSingleton<ISunriseSunset, SunriseSunset>()
            .AddSingleton<IEnvoyClientFactory, EnvoyClientFactory>()
            .AddPvOutputClient()
            .AddHomeAssistantApi()
            .AddTransient<INetFrequencyReader, HomeAssistant>()
            .AddTransient<IInverterDataReader, EnvoyReader>()
            .AddTransient<IOutputWriter, PvOutputWriter>()
            .AddTransient<IPipeline, Pipeline>()
            .AddHostedService<EnvoyReaderService>();
    })
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var env = hostingContext.HostingEnvironment;
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false);

        config.AddEnvironmentVariables("ENVOYREADER_");

        if (hostingContext.HostingEnvironment.IsDevelopment())
        {
            config.AddUserSecrets<Program>();
        }
    })
    .Build();

await host.RunAsync();
