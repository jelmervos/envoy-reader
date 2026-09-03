using Microsoft.Extensions.DependencyInjection;

internal static class SettingsServiceExtenstions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
    {
        services.AddOptions<EnvoyClientSettings>().BindConfiguration(ConfigSections.EnvoyClient);
        services.AddOptions<PvOutputSettings>().BindConfiguration(ConfigSections.PvOutput);
        services.AddOptions<SystemLocationSettings>().BindConfiguration(ConfigSections.SystemLocation);
        services.AddOptions<HomeAssistantSettings>().BindConfiguration(ConfigSections.HomeAssistant);

        return services;
    }
}
