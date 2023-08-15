using FluentValidation;
using Microsoft.Extensions.Options;

namespace AzureLogging.Options.Validators; 

/// <summary>
/// Provides FluentValidation based abstract class to build <see cref="IValidateOptions{TOptions}"/> implementation
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AbstractOptionsValidator<T> : AbstractValidator<T>, IValidateOptions<T> where T : class {
	/// <inheritdoc cref="IValidateOptions{TOptions}.Validate"/>
	public virtual ValidateOptionsResult Validate(string? name, T options) {
		var result = Validate(options);

		return result.IsValid 
			? ValidateOptionsResult.Success
			: ValidateOptionsResult.Fail(string.Join("; ", result.Errors.Select(e => e.ErrorMessage)));
	}
}
