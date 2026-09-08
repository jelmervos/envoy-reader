using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(State))]
internal partial class HomeAssistantSerializerContext : JsonSerializerContext
{
}
