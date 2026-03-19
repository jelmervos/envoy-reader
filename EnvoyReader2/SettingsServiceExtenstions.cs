using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal static class SettingsServiceExtenstions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, HostBuilderContext hostContext)
    {
        services.AddOptions<EnvoyClientSettings>().Bind(hostContext.Configuration.GetRequiredSection(ConfigSections.EnvoyClient));
        services.AddOptions<PvOutputSettings>().Bind(hostContext.Configuration.GetRequiredSection(ConfigSections.PvOutput));
        services.AddOptions<SystemLocationSettings>().Bind(hostContext.Configuration.GetSection(ConfigSections.SystemLocation));
        services.AddOptions<HomeAssistantSettings>().Bind(hostContext.Configuration.GetSection(ConfigSections.HomeAssistant));

        return services;
    }
}
