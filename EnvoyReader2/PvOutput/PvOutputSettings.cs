using System.ComponentModel.DataAnnotations;

internal class PvOutputSettings
{
    [Required(AllowEmptyStrings = false)]
    public required string ApiKey { get; set; }
    public int SystemId { get; set; }
}
