using AzureLogging.Logging.Contracts;

namespace AzureLogging.Logging.Request;

/// <summary>
/// Adds logging request headers
/// </summary>
public class RequestHeadersDiagnosticItem : IDiagnosticContextItem {
	/// <inheritdoc />
	public object GetData(HttpContext context) => context.Request.Headers.ToArray();

	/// <inheritdoc />
	public string Key => "Headers";
}