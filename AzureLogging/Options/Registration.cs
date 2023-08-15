using AzureLogging.Extensions;
using AzureLogging.Names;
using AzureLogging.Options.Validators;

namespace AzureLogging.Options; 

/// <summary>
/// Dependency container registration for app-specific options 
/// </summary>
public static class Registration {
	/// <summary>
	/// Registers default configuration classes
	/// </summary>
	/// <param name="services"><see cref="IServiceCollection"/> instance</param>
	/// <param name="configuration"><see cref="IConfiguration"/> instance</param>
	/// <returns><see cref="IServiceCollection"/> instance</returns>
	public static IServiceCollection AddDefaultOptions(this IServiceCollection services, IConfiguration configuration) =>
		services
			.AddOptions()
			.ConfigureWithValidation<LogConfigOptions, LogConfigOptionsValidator>(configuration.GetSection(JsonConfigKeys.LogConfig));
}