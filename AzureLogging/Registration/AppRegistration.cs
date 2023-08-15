using AzureLogging.Models;

namespace AzureLogging.Registration; 

/// <summary>
/// Application specific services setup
/// </summary>
public static class AppRegistration {
	/// <summary>
	/// Configures dependency injection with application specific services
	/// </summary>
	/// <param name="services"><see cref="IServiceCollection"/></param>
	/// <returns><see cref="IServiceCollection"/></returns>
	public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
		services.AddSingleton<SampleRepository>();
		return services;
	}
}