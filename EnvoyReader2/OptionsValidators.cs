using Microsoft.Extensions.Options;

[OptionsValidator]
internal partial class ValidateEnvoyClientSettings : IValidateOptions<EnvoyClientSettings>
{
}

[OptionsValidator]
internal partial class ValidatePvOutputSettings : IValidateOptions<PvOutputSettings>
{
}

[OptionsValidator]
internal partial class ValidateServiceSettings : IValidateOptions<ServiceSettings>
{
}