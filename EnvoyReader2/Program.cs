using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .UseContentRoot(Utilities.GetStartupFolder())
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
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(builder => builder.AddConsole())
            .AddSingleton<IClock, Clock>();
    });


var rootCommand = new RootCommand();
rootCommand.SetAction((parseResult) => EnvoyReaderServiceConfigurator.ConfigureServices(hostBuilder));

var healthcheckCommand = new Command("healthcheck");
healthcheckCommand.SetAction((parseResult) => HealthcheckServiceConfigurator.ConfigureServices(hostBuilder));
rootCommand.Subcommands.Add(healthcheckCommand);

rootCommand.Parse(args).Invoke();

using var host = hostBuilder.Build();
await host.RunAsync();

