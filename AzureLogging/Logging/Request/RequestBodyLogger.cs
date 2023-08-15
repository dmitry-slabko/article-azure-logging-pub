using AzureLogging.Logging.Contracts;

namespace AzureLogging.Logging.Request; 

/// <summary>
/// Implements <see cref="IDiagnosticContextItem"/> to provide a request body for logging
/// </summary>
public sealed class RequestBodyDiagnosticItem : IDiagnosticContextItem {
	private string requestBody = "";

	/// <summary>
	/// Stores request body for logging
	/// </summary>
	/// <param name="body">Deserialized request body</param>
	public void ProvideRequestBody(string body) {
		requestBody = body;
	}

	/// <inheritdoc />
	public string Key => "Body";

	/// <inheritdoc />
	public object GetData(HttpContext context) => requestBody;
}