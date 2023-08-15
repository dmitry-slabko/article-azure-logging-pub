using AzureLogging.Options;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Settings.Configuration;
using ILogger = Serilog.ILogger;

namespace AzureLogging.Extensions; 

/// <summary>
/// Extension methods to assist during WebApplicationBuilder configuration
/// </summary>
public static class ConfigurationExtensions {
	/// <summary>
	/// Adds default configuration providers (appSettings.json files, Azure Configuration, environment variables)
	/// </summary>
	/// <param name="configuration"><see cref="IConfigurationBuilder"/> instance</param>
	/// <param name="services"><see cref="IServiceCollection"/> instance</param>
	/// <param name="environment"><see cref="IHostEnvironment"/> instance</param>
	public static void SetupDefaultConfiguration(this IConfigurationBuilder configuration, IServiceCollection services, IHostEnvironment environment) {
		configuration
			.AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"appSettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

		if (environment.IsDevelopment()) {
			// this file is not stored in the repository, as it contains development environment specific secrets
			configuration.AddJsonFile("appSettings.Secrets.json", optional: true, reloadOnChange: true);
		}

		configuration.AddEnvironmentVariables();

		services.AddAzureAppConfiguration();
	}

	/// <summary>
	/// Creates a bootstrap logger to use during application initialization and startup
	/// </summary>
	/// <param name="configuration"><see cref="IConfiguration"/> instance</param>
	/// <param name="bootstrapSectionName">Config section name to read the bootstrap Serilog configuration from</param>
	/// <returns>ILogger instance set up using bootstrap configuration</returns>
	public static ILogger CreateBootstrapLogger(this IConfiguration configuration, string bootstrapSectionName) {
		var bootstrapOptions = configuration.GetSection(bootstrapSectionName).Get<LogConfigOptions>() ?? new LogConfigOptions();

		var readerOptions = new ConfigurationReaderOptions {
			SectionName = bootstrapOptions.BootstrapConfigurationSectionName
		};

		return new LoggerConfiguration()
			.ReadFrom
			.Configuration(configuration, readerOptions)
			.CreateBootstrapLogger();
	}

	/// <summary>
	/// Adds a configuration options of type T with options validator implementing IValidateOptions&lt;T&gt;
	/// </summary>
	/// <typeparam name="T">Options class</typeparam>
	/// <typeparam name="TValidator">Class implementing <see cref="IValidateOptions{TOptions}"/></typeparam>
	/// <param name="services"><see cref="IServiceCollection"/> instance</param>
	/// <param name="configuration"><see cref="IConfigurationSection"/> instance</param>
	/// <returns>Passed <see cref="IServiceCollection"/> instance</returns>
	public static IServiceCollection ConfigureWithValidation<T, TValidator>(this IServiceCollection services, IConfigurationSection configuration)
		where T : class, new()
		where TValidator : class, IValidateOptions<T>, new() =>
		services
			.Configure<T>(configuration)
			.AddTransient<IValidateOptions<T>, TValidator>();
}