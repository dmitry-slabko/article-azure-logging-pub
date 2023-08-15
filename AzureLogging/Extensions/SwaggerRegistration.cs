using System.Reflection;
using Microsoft.OpenApi.Models;

namespace AzureLogging.Extensions; 

/// <summary>
/// Configures Swagger
/// </summary>
public static class SwaggerRegistration {
	/// <summary>
	/// Adds required Swagger services and setup
	/// </summary>
	/// <param name="services"><see cref="IServiceCollection"/></param>
	/// <param name="environment"><see cref="IHostEnvironment"/></param>
	/// <returns><see cref="IServiceCollection"/></returns>
	public static IServiceCollection AddSwaggerServices(this IServiceCollection services, IHostEnvironment environment) {
		if (environment.IsDevelopment()) {
			services
				.AddEndpointsApiExplorer()
				.AddSwaggerGen(options => {
					options.EnableAnnotations();
					options.SwaggerDoc("v1.0",
						new OpenApiInfo {
							Version = "v1.0",
							Title = "Sample API"
						});
					options.UseInlineDefinitionsForEnums();
					var xmlFilename = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
					options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
				});
		}

		return services;
	}

	/// <summary>
	/// Prepares Swagger page for the application
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static WebApplication UseSwaggerPage(this WebApplication app) {
		if (app.Environment.IsDevelopment()) {
			app
				.UseSwagger(setupAction: null)
				.UseSwaggerUI(options => {
					options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1.0");
					options.RoutePrefix = "";
				});
		}

		return app;
	}
}