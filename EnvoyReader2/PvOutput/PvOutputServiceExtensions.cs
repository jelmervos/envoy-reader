using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[OptionsValidator]
internal partial class ValidatePvOutputSettings : IValidateOptions<PvOutputSettings>
{
}

internal static class PvOutputServiceExtensions
{
    public static IServiceCollection AddPvOutputOptions(this IServiceCollection services)
    {
        services.AddOptions<PvOutputSettings>()
                .BindConfiguration(ConfigSections.PvOutput);
        services.AddSingleton<IValidateOptions<PvOutputSettings>, ValidatePvOutputSettings>();

        return services;
    }
}