using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

internal static class HomeAssistantServiceExtensions
{
    public static IServiceCollection AddHomeAssistantApi(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var homeAssistantSettings = provider.GetRequiredService<IOptions<HomeAssistantSettings>>();
        services.AddRefitGeneratedClient<IHomeAssistantApi>(new RefitSettings
        {
            AuthorizationHeaderValueGetter = (request, cancelToken) => ValueTask.FromResult(homeAssistantSettings.Value.Token)
        }).ConfigureHttpClient(c => c.BaseAddress = new Uri(homeAssistantSettings.Value.Address));

        return services;
    }
}