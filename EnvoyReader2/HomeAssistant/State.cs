using System.Text.Json.Serialization;

internal readonly record struct State
{
    [JsonPropertyName("entity_id")]
    public required string EntityId { get; init; }
    
    [JsonPropertyName("state")]
    public required string StateValue { get; init; }
    
    [JsonPropertyName("last_updated")]
    public required DateTimeOffset LastUpdated { get; init; }

    [JsonPropertyName("last_changed")]
    public required DateTimeOffset LastChanged { get; init; }

}
