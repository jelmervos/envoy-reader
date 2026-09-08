using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using System.Text.Json;

internal static class HomeAssistantServiceExtensions
{
    public static IServiceCollection AddHomeAssistantApi(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var homeAssistantSettings = provider.GetRequiredService<IOptions<HomeAssistantSettings>>();
        services.AddRefitGeneratedClient<IHomeAssistantApi>(new RefitSettings
        {
            ContentSerializer = GetContentSerializer(),
            AuthorizationHeaderValueGetter = (request, cancelToken) => ValueTask.FromResult(homeAssistantSettings.Value.Token)
        }).ConfigureHttpClient(c => c.BaseAddress = new Uri(homeAssistantSettings.Value.Address));

        return services;
    }

    private static SystemTextJsonContentSerializer GetContentSerializer() =>
        new(new JsonSerializerOptions
        {
            TypeInfoResolver = HomeAssistantSerializerContext.Default
        });
}