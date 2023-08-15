using AzureLogging.Endpoints;
using AzureLogging.Exceptions;
using AzureLogging.Extensions;
using AzureLogging.Logging.Middleware;
using AzureLogging.Logging.Registration;
using AzureLogging.Options;
using AzureLogging.Registration;

namespace AzureLogging; 

/// <summary>
/// Implements 'standard' startup sequence
/// </summary>
public class Startup {
	private readonly IConfiguration configuration;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="configuration"><see cref="IConfiguration"/></param>
	public Startup(IConfiguration configuration) {
		this.configuration = configuration;
	}

	/// <summary>
	/// Configures dependency injection container
	/// </summary>
	/// <param name="services"><see cref="IServiceCollection"/></param>
	/// <param name="environment"><see cref="IHostEnvironment"/></param>
	public void ConfigureServices(IServiceCollection services, IHostEnvironment environment) {
		services
			.AddHttpClient()
			.AddDefaultOptions(configuration)
			.AddHttpContextAccessor()
			.AddApplicationServices();

		services
			.AddAuthorization()
			.AddAuthentication();

		services.AddSwaggerServices(environment);
	}

	/// <summary>
	/// Configures the application
	/// </summary>
	/// <param name="app"><see cref="WebApplication"/></param>
	public void Configure(WebApplication app) {
		if (!app.Environment.IsDevelopment()) {
			app
				.UseHsts()
				.UseHttpsRedirection()
				.UseAzureAppConfiguration();
		}

		app.RegisterEndpoints();

		app
			.UseAuthorization()
			.UseAuthentication()
			.UseAppInsightsLoggingWithSerilog()
			.UseMiddleware<CorrelationIdSubstitutionMiddleware>()
			.UseExceptionHandler(builder => builder.Run(DefaultExceptionHandler.HandleExceptionAsync));

		app.UseSwaggerPage();
	}
}