using Serilog.Events;

namespace AzureLogging.Options; 

/// <summary>
/// Class with basic options for logging setup
/// </summary>
public sealed class LogConfigOptions {
	/// <summary>
	/// Configuration section name where Serilog config is stored; the default value is "SerilogBootstrap"
	/// </summary>
	public string BootstrapConfigurationSectionName { get; init; } = "SerilogBootstrap";

	/// <summary>
	/// Configuration section name where Serilog config is stored; the default value is "Serilog"
	/// </summary>
	public string ConfigurationSectionName { get; init; } = "Serilog";

	/// <summary>
	/// Flag to log HTTP requests; the default value is false
	/// </summary>
	public bool HttpRequestLogging { get; init; }

	/// <summary>
	/// Flag to enable logging to Azure file system (under d:\home\logFiles); the default value is false
	/// </summary>
	public bool LogToAzureFileSystem { get; init; }

	/// <summary>
	/// Log level for logging HTTP requests; the default value is <see cref="LogEventLevel.Debug"/>
	/// </summary>
	public LogEventLevel HttpRequestLogLevel { get; init; } = LogEventLevel.Debug;

	/// <summary>
	/// Azure filesystem storage file size limit in bytes; the default value is 4 MB 
	/// </summary>
	public long? AzureFileSizeLimit { get; init; } = 4 * 1024 * 1024;

	/// <summary>
	/// Roll Azure filesystem storage log file on reaching the size limit; the default value is true
	/// </summary>
	public bool AzureRollOnSizeLimit { get; init; } = true;

	/// <summary>
	/// Retained file count on Azure filesystem storage; the default value is 1
	/// </summary>
	public int? AzureRetainedFileCount { get; init; } = 1;

	/// <summary>
	/// How long the logs are kept on Azure filesystem storage;
	/// setting to null retains logs until they are overwritten due to reaching the size limit constraint.
	/// The default value is 1 day.
	/// </summary>
	public TimeSpan? AzureRetainTimeLimit { get; init; } = TimeSpan.FromDays(1);
}