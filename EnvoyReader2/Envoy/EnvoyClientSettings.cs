internal record EnvoyClientSettings
{
    public required string Host { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string TokenFile { get; init; }
}
