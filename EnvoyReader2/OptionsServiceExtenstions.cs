using Microsoft.Extensions.DependencyInjection;

internal static class OptionsServiceExtenstions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
    {
        services.AddOptions<EnvoyClientSettings>().BindConfiguration(ConfigSections.EnvoyClient).ValidateDataAnnotations();
        services.AddOptions<PvOutputSettings>().BindConfiguration(ConfigSections.PvOutput).ValidateDataAnnotations();
        services.AddOptions<SystemLocationSettings>().BindConfiguration(ConfigSections.SystemLocation).ValidateDataAnnotations();
        services.AddOptions<HomeAssistantSettings>().BindConfiguration(ConfigSections.HomeAssistant).ValidateDataAnnotations();
        services.AddOptions<ServiceSettings>().BindConfiguration(ConfigSections.Service).ValidateDataAnnotations();

        return services;
    }
}  
