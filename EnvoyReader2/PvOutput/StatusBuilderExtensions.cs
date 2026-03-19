using PVOutput.Net.Builders;
using PVOutput.Net.Objects;

internal static class StatusBuilderExtensions
{
    public static StatusPostBuilder<IStatusPost> SetVoltageIfNotNull(this StatusPostBuilder<IStatusPost> builder, decimal? value)
    {
        if (value != null)
        {
            return builder.SetVoltage(Math.Round(value.Value, 2));
        }

        return builder;
    }

    public static StatusPostBuilder<IStatusPost> SetTemperatureIfNotNull(this StatusPostBuilder<IStatusPost> builder, decimal? value)
    {
        if (value != null)
        {
            return builder.SetTemperature(Math.Round(value.Value, 2));
        }

        return builder;
    }
}
