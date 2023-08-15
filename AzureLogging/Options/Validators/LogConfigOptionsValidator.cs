using FluentValidation;
using Microsoft.Extensions.Options;

namespace AzureLogging.Options.Validators; 

/// <summary>
/// Implements <see cref="IValidateOptions{TOptions}"/> for <see cref="LogConfigOptions"/>
/// via <see cref="AbstractOptionsValidator{T}"/> and FluentValidation rules
/// </summary>
public sealed class LogConfigOptionsValidator : AbstractOptionsValidator<LogConfigOptions> {
	/// <summary>
	/// Constructor that defines validation rules
	/// </summary>
	public LogConfigOptionsValidator() {
		RuleFor(x => x.ConfigurationSectionName)
			.NotEmpty()
			.WithMessage("Serilog configuration section name cannot be empty");

		RuleFor(x => x.BootstrapConfigurationSectionName)
			.NotEmpty()
			.WithMessage("Serilog bootstrap configuration section name cannot be empty");

		RuleFor(x => x.AzureFileSizeLimit)
			.Must(x => x > 0)
			.WithMessage($"When specified, {nameof(LogConfigOptions.AzureFileSizeLimit)} must be a positive value");

		RuleFor(x => x.AzureRetainedFileCount)
			.Must(x => x > 0)
			.WithMessage($"When specified, {nameof(LogConfigOptions.AzureRetainedFileCount)} must be a positive value");

		RuleFor(x => x.AzureRetainTimeLimit)
			.Must(x => x > TimeSpan.Zero)
			.WithMessage($"When specified, {nameof(LogConfigOptions.AzureRetainTimeLimit)} must specify a non-negative time span value");
	}
}