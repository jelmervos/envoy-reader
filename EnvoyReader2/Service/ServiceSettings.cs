using System.ComponentModel.DataAnnotations;

internal sealed class ServiceSettings
{
    [property: Range(2, 60)]
    public int AggregatorIntervalInMinutes { get; set; }
}
