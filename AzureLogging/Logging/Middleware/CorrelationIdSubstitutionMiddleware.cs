using AzureLogging.Names;

namespace AzureLogging.Logging.Middleware; 

/// <summary>
/// Provides middleware to add a correlation id to a request if it does not have one
/// </summary>
public sealed class CorrelationIdSubstitutionMiddleware {
	private readonly RequestDelegate next;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="next"><see cref="RequestDelegate"/></param>
	public CorrelationIdSubstitutionMiddleware(RequestDelegate next) {
		this.next = next;
	}

	/// <summary>
	/// Middleware request processing method
	/// </summary>
	/// <param name="context"><see cref="HttpContext"/></param>
	public async Task InvokeAsync(HttpContext context) {
		if (!context.Request.Headers.TryGetValue(HeaderKeyNames.CorrelationId, out var header)) {
			var correlationId = Guid.NewGuid().ToString();
			context.Request.Headers[HeaderKeyNames.CorrelationId] = correlationId;
			context.Response.Headers[HeaderKeyNames.CorrelationId] = correlationId;
		}

		await next(context);
	}
}