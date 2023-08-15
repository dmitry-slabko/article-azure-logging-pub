using AzureLogging.Extensions;
using AzureLogging.Logging.Contracts;
using AzureLogging.Logging.Request;
using AzureLogging.Names;
using AzureLogging.Options;
using AzureLogging.Options.Validators;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Settings.Configuration;

namespace AzureLogging.Logging.Registration; 

/// <summary>
/// Adds extenstion methods for <see cref="IHostBuilder"/> to configure a host builder
/// for Serilog and AppInsights logging
/// </summary>
public static class HostBuilderExtensions {
	/// <summary>
	/// Adds logging to Application Insights and Azure Log Stream using Serilog;
	/// also applies any required custom configuration to Serilog, such as logging to local files, etc. 
	/// </summary>
	/// <param name="host"><see cref="IHostBuilder"/> instance</param>
	/// <param name="logConfigSection"><see cref="IConfigurationSection"/> that holds <see cref="LogConfigOptions"/> data</param>
	/// <returns><see cref="IHostBuilder"/> instance</returns>
	public static IHostBuilder AddAppInsightsLoggingWithSerilog(this IHostBuilder host, IConfigurationSection logConfigSection) {
		var options = logConfigSection.Get<LogConfigOptions>() ?? new LogConfigOptions();

		host.ConfigureServices(services => {
			services
				.ConfigureWithValidation<LogConfigOptions, LogConfigOptionsValidator>(logConfigSection)
				.AddApplicationInsightsTelemetry();

			services.AddHeaderPropagation(x => {
				x.Headers.Add(HeaderKeyNames.CorrelationId);
				x.Headers.Add(HeaderKeyNames.TraceContext);
			});

			if (options.HttpRequestLogging) {
				services
					.AddScoped<RequestBodyDiagnosticItem>()
					.AddTransient<IDiagnosticContextItem>(provider => provider.GetRequiredService<RequestBodyDiagnosticItem>())
					.AddScoped<IDiagnosticContextItem, RequestHeadersDiagnosticItem>()
					.AddScoped<IDiagnosticContextItem, RequestIpDiagnosticItem>();
			}
		});

		host.ConfigureLogging(logging => {
			logging.ClearProviders();
			if (options.LogToAzureFileSystem) {
				logging.AddAzureWebAppDiagnostics();
			}
		});

		host.UseSerilog(SetupLoggerConfiguration, writeToProviders: false, preserveStaticLogger: false);

		return host;
	}

	private static void SetupLoggerConfiguration(HostBuilderContext context, IServiceProvider provider, LoggerConfiguration loggerConfiguration) {
		var options = provider.GetRequiredService<IOptions<LogConfigOptions>>().Value;

		loggerConfiguration
			.Enrich.FromLogContext()
			.Enrich.WithCorrelationIdHeader(HeaderKeyNames.CorrelationId)
			.Enrich.WithExceptionDetails()
			.Enrich.WithSensitiveDataMasking(x => {
				x.Mode = MaskingMode.InArea;
			});

		if (options.HttpRequestLogging) {
			// this line is required 'as is' in order to enable HTTP request logging;
			// see more here: https://github.com/serilog/serilog-aspnetcore#request-logging
			loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);
		}

		var readerOptions = new ConfigurationReaderOptions {
			SectionName = options.ConfigurationSectionName
		};

		loggerConfiguration
			.ReadFrom.Configuration(context.Configuration, readerOptions)
			.ReadFrom.Services(provider);

		// Configure Application Insights sink
		var telemetryConfiguration = provider.GetService<IOptions<TelemetryConfiguration>>();
		if (!string.IsNullOrEmpty(telemetryConfiguration?.Value.ConnectionString)) {
			// We have a valid Application Insights setup
			loggerConfiguration
				.WriteTo
				.ApplicationInsights(telemetryConfiguration.Value, TelemetryConverter.Traces);
		}

		// Configure Azure log stream sink
		if (options.LogToAzureFileSystem && !context.HostingEnvironment.IsDevelopment()) {
			loggerConfiguration
				.WriteTo
				.Async(x => x.File(
					$@"D:\home\LogFiles\Application\{context.HostingEnvironment.ApplicationName}.txt",
					fileSizeLimitBytes: options.AzureFileSizeLimit,
					rollOnFileSizeLimit: options.AzureRollOnSizeLimit,
					retainedFileCountLimit: options.AzureRetainedFileCount,
					retainedFileTimeLimit: options.AzureRetainTimeLimit,
					shared: true,
					flushToDiskInterval: TimeSpan.FromSeconds(1)
				));
		}
	}
}