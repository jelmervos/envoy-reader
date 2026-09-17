using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PVOutput.Net;

[OptionsValidator]
internal partial class ValidatePvOutputSettings : IValidateOptions<PvOutputSettings>
{
}

internal static class PvOutputServiceExtensions
{
    public static IServiceCollection AddPvOutputClient(this IServiceCollection services)
    {
        services.AddTransient<IPVOutputClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<PvOutputSettings>>();
            return new PVOutputClient(settings.Value.ApiKey, settings.Value.SystemId);
        });

        return services;
    }

    public static IServiceCollection AddPvOutputOptions(this IServiceCollection services)
    {
        services.AddOptions<PvOutputSettings>()
                .BindConfiguration(ConfigSections.PvOutput);
        services.AddSingleton<IValidateOptions<PvOutputSettings>, ValidatePvOutputSettings>();

        return services;
    }
}