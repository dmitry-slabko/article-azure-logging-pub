using System.Net;
using AzureLogging.Logging.Contracts;

namespace AzureLogging.Logging.Request; 

/// <summary>
/// Adds logging client IP address
/// </summary>
public sealed class RequestIpDiagnosticItem : IDiagnosticContextItem{
	/// <inheritdoc />
	public object GetData(HttpContext context) => context.Request.HttpContext.Connection.RemoteIpAddress ?? IPAddress.None;

	/// <inheritdoc />
	public string Key => "IP address";
}