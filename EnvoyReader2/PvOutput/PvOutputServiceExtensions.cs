using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PVOutput.Net;

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
}