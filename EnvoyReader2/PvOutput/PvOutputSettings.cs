using System.ComponentModel.DataAnnotations;

internal sealed class PvOutputSettings
{
    [Required(AllowEmptyStrings = false)]
    public required string ApiKey { get; set; }
    public int SystemId { get; set; }
}
