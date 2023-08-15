using System.Text;
using AzureLogging.Logging.Request;
using AzureLogging.Options;
using Microsoft.Extensions.Options;

namespace AzureLogging.Logging.Middleware; 

/// <summary>
/// Provides middleware to log request body
/// </summary>
public sealed class RequestBodyLoggingMiddleware {
	private readonly RequestDelegate next;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="next"><see cref="RequestDelegate"/></param>
	public RequestBodyLoggingMiddleware(RequestDelegate next) {
		this.next = next;
	}

	/// <summary>
	/// Middleware request processing method
	/// </summary>
	/// <param name="context"><see cref="HttpContext"/></param>
	/// <param name="logOptions"><see cref="IOptions{TOptions}"/> for <see cref="LogConfigOptions"/></param>
	public async Task InvokeAsync(HttpContext context, IOptions<LogConfigOptions> logOptions) {
		if (logOptions.Value.HttpRequestLogging && (context.Request.Method == HttpMethod.Post.Method
			|| context.Request.Method == HttpMethod.Put.Method || context.Request.Method == HttpMethod.Patch.Method)) {
			string body = await TryGetRequestData(context.Request);
			if (!string.IsNullOrEmpty(body)) {
				var bodyLogger = context.RequestServices.GetRequiredService<RequestBodyDiagnosticItem>();
				bodyLogger.ProvideRequestBody(body);
			}
		}

		await next(context);
	}

	private static async ValueTask<string> TryGetRequestData(HttpRequest request) {
		if (request.ContentLength.GetValueOrDefault() == 0) {
			return "";
		}

		if (request.HasFormContentType) {
			return ReadFormData(request.Form);
		}

		return await ReadBodyAsync(request);
	}

	private static async ValueTask<string> ReadBodyAsync(HttpRequest request) {
		if (!request.Body.CanSeek) {
			request.EnableBuffering();
		}

		using var streamReader = new StreamReader(request.Body, leaveOpen: true);
		string body = await streamReader.ReadToEndAsync();

		// Reset the request's body stream position for the next middleware in the pipeline.
		request.Body.Position = 0;
		return body;
	}

	private static string ReadFormData(IFormCollection form) {
		// Note: this implementation does not deal with submitted file data.
		var sb = new StringBuilder();
		foreach (var item in form) {
			if (sb.Length > 0) {
				sb.AppendLine();
			}

			sb.Append(item.Key).Append('=').Append(item.Value);
		}

		return sb.ToString();
	}
}