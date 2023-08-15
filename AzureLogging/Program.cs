using System.Diagnostics;
using AzureLogging.Exceptions;
using AzureLogging.Extensions;
using AzureLogging.Logging.Registration;
using AzureLogging.Names;
using Serilog;
using Serilog.Debugging;

namespace AzureLogging; 

/// <summary>
/// Program startup class
/// </summary>
public class Program {
	/// <summary>
	/// Main entry point
	/// </summary>
	/// <param name="args">Arguments</param>
	public static async Task Main(string[] args) {
		SerilogSelfLogging();
		CreateBootstrapLogger();

		try {
			var builder = WebApplication.CreateBuilder(args);
			builder.Configuration.SetupDefaultConfiguration(builder.Services, builder.Environment);
			builder.Host.AddAppInsightsLoggingWithSerilog(builder.Configuration.GetSection(JsonConfigKeys.LogConfig));

			var startup = new Startup(builder.Configuration);
			startup.ConfigureServices(builder.Services, builder.Environment);

			var app = builder.Build();
			startup.Configure(app);

			await app.RunAsync();
		} catch (Exception ex) {
			Log.Fatal(ex, "");
			Console.WriteLine(ex.Unwrap());
		} finally {
			await Log.CloseAndFlushAsync();
		}
	}

	private static void CreateBootstrapLogger() {
		var bootstrapConfiguration = new ConfigurationBuilder()
			.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false)
			.AddEnvironmentVariables()
			.Build();

		Log.Logger = bootstrapConfiguration.CreateBootstrapLogger(JsonConfigKeys.LogConfig);
	}

	[Conditional("DEBUG")]
	private static void SerilogSelfLogging() {
		SelfLog.Enable(Console.WriteLine);
	}
}