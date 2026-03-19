internal record HomeAssistantSettings
{
    public required string Token { get; init; }
    public required string Address { get; init; }
    public required string NetFreqEntityId { get; init; }
}
