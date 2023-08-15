using AzureLogging.Logging.Contracts;
using AzureLogging.Logging.Middleware;
using AzureLogging.Options;
using Microsoft.Extensions.Options;
using Serilog;

namespace AzureLogging.Logging.Registration; 

/// <summary>
/// Adds extenstion methods for <see cref="IApplicationBuilder"/> to use Serilog and AppInsights logging
/// </summary>
public static class ApplicationBuilderExtensions {
	/// <summary>
	/// Configures HTTP request logging into Application Insights;
	/// this call must be preceded with a call to <see cref="HostBuilderExtensions"/>.AddAppInsightsLogging method.
	/// This method uses <see cref="LogConfigOptions"/> configuration data.
	/// </summary>
	/// <param name="app"><see cref="IApplicationBuilder"/> instance</param>
	/// <returns><see cref="IApplicationBuilder"/> instance</returns>
	public static IApplicationBuilder UseAppInsightsLoggingWithSerilog(this IApplicationBuilder app) {
		var logOptions = app.ApplicationServices.GetRequiredService<IOptions<LogConfigOptions>>().Value;

		if (logOptions.HttpRequestLogging) {
			app.UseSerilogRequestLogging(options => {
				options.GetLevel = (_, _, _) => logOptions.HttpRequestLogLevel;

				options.EnrichDiagnosticContext = (diagnosticContext, httpContext) => {
					foreach (var filler in httpContext.RequestServices.GetServices<IDiagnosticContextItem>()) {
						var data = filler.GetData(httpContext);
						if (data is not null) {
							diagnosticContext.Set(filler.Key, data, destructureObjects: true);
						}
					}
				};
			});

			app.UseMiddleware<RequestBodyLoggingMiddleware>();
		}

		app.UseHeaderPropagation();

		return app;
	}

}