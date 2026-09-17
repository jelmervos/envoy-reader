using Microsoft.Extensions.DependencyInjection;

internal static class HealthcheckServiceExtenstions
{
    public static IServiceCollection AddHealthcheckOptions(this IServiceCollection services)
    {
        services.AddOptions<HealthcheckSettings>()
                .BindConfiguration(ConfigSections.Healthcheck);

        return services;
    }
}
