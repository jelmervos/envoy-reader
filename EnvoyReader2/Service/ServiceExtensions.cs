using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[OptionsValidator]
internal partial class ValidateServiceSettings : IValidateOptions<ServiceSettings>
{
}

internal static class ServiceExtenstions
{
    public static IServiceCollection AddServiceOptions(this IServiceCollection services)
    {
        services.AddOptions<ServiceSettings>()
                .BindConfiguration(ConfigSections.Service);
        services.AddSingleton<IValidateOptions<ServiceSettings>, ValidateServiceSettings>();

        return services;
    }
}
