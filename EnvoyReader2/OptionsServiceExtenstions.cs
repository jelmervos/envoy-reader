using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

internal static class OptionsServiceExtenstions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
    {
        services.AddOptions<EnvoyClientSettings>()
                .BindConfiguration(ConfigSections.EnvoyClient);
        services.AddSingleton<IValidateOptions<EnvoyClientSettings>, ValidateEnvoyClientSettings>();

        services.AddOptions<PvOutputSettings>()
                .BindConfiguration(ConfigSections.PvOutput);
        services.AddSingleton<IValidateOptions<PvOutputSettings>, ValidatePvOutputSettings>();

        services.AddOptions<SystemLocationSettings>()
                .BindConfiguration(ConfigSections.SystemLocation);

        services.AddOptions<HomeAssistantSettings>()
                .BindConfiguration(ConfigSections.HomeAssistant);

        services.AddOptions<ServiceSettings>()
                .BindConfiguration(ConfigSections.Service);
        services.AddSingleton<IValidateOptions<ServiceSettings>, ValidateServiceSettings>();

        return services;
    }
}  
