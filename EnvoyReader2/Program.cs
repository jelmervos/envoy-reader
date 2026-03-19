using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host.CreateDefaultBuilder(args)
    .UseContentRoot(Utilities.GetStartupFolder())
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(builder => builder.AddConsole())
            .AddApplicationOptions(hostContext)
            .AddSingleton<IClock, Clock>()
            .AddSingleton<ISunriseSunset, SunriseSunset>()
            .AddSingleton<IEnvoyClientFactory, EnvoyClientFactory>()
            .AddPvOutputClient()
            .AddHomeAssistantApi()
            .AddTransient<INetFrequencyReader, HomeAssistant>()
            .AddTransient<IInverterDataReader, EnvoyReader>()
            .AddTransient<IOutputWriter, PvOutputWriter>()
            .AddTransient<IPipeline, Pipeline>();
    })
    .Build();

var pipeline = host.Services.GetRequiredService<IPipeline>();
await pipeline.Start();
