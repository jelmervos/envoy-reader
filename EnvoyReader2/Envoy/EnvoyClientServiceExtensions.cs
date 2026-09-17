using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[OptionsValidator]
internal partial class ValidateEnvoyClientSettings : IValidateOptions<EnvoyClientSettings>
{
}

internal static class EnvoyClientServiceExtensions
{
    public static IServiceCollection AddEnvoyOptions(this IServiceCollection services)
    {
        services.AddOptions<EnvoyClientSettings>()
                .BindConfiguration(ConfigSections.EnvoyClient);
        services.AddSingleton<IValidateOptions<EnvoyClientSettings>, ValidateEnvoyClientSettings>();

        return services;
    }
}
