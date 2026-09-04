using System.ComponentModel.DataAnnotations;

internal class ServiceSettings
{
    [property: Range(2, 60)]
    public required int PipelineIntervalInMinutes { get; set; }
}
