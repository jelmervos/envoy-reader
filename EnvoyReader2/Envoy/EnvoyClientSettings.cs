using System.ComponentModel.DataAnnotations;

internal class EnvoyClientSettings
{
    [Required(AllowEmptyStrings = false)]
    public required string Host { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Username { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Password { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string TokenFile { get; set; }
}
