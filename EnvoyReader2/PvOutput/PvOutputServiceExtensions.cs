using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[OptionsValidator]
internal partial class ValidatePvOutputSettings : IValidateOptions<PvOutputSettings>
{
}

internal static class PvOutputServiceExtensions
{
    private const string PvOutputBaseAddress = "https://pvoutput.org/";

    public static IServiceCollection AddPvOutputOptions(this IServiceCollection services)
    {
        services.AddOptions<PvOutputSettings>()
                .BindConfiguration(ConfigSections.PvOutput);
        services.AddSingleton<IValidateOptions<PvOutputSettings>, ValidatePvOutputSettings>();

        return services;
    }

    public static IServiceCollection AddPvOutputClient(this IServiceCollection services)
    {
        services.AddHttpClient<IOutputWriter, PvOutputWriter>(client =>
        {
            client.BaseAddress = new Uri(PvOutputBaseAddress);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}   