namespace AzureLogging.Names; 

/// <summary>
/// HTTP request header key names
/// </summary>
public class HeaderKeyNames {
	/// <summary>
	/// Trace context header key
	/// </summary>
	public const string TraceContext = "traceparent";

	/// <summary>
	/// Correlation ID header key
	/// </summary>
	public static string CorrelationId => "x-correlation-id";
}