using Microsoft.Extensions.DependencyInjection;

internal static class SunriseSunsetServiceExtensions
{
    public static IServiceCollection AddSunriseSunsetOptions(this IServiceCollection services)
    {
        services.AddOptions<SystemLocationSettings>()
                .BindConfiguration(ConfigSections.SystemLocation);

        return services;
    }
}
