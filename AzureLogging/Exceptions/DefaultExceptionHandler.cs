using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace AzureLogging.Exceptions; 

/// <summary>
/// Implements a request exception handler middleware to log exception info
/// </summary>
internal static class DefaultExceptionHandler {
	/// <summary>
	/// Exception handler middleware method
	/// </summary>
	/// <param name="context"></param>
	public static async Task HandleExceptionAsync(HttpContext context) {
		var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

		if (exception == null) {
			return;
		}

		var logger = context.RequestServices.GetRequiredService<ILogger>();
		logger.Error(exception, "");

		var exceptionMatchers = context.RequestServices.GetServices<Func<Exception, ProblemDetails?>>();
		var problem = exceptionMatchers
			.Select(x => x(exception))
			.Where(x => x is not null)
			.FirstOrDefault(new ProblemDetails {
				Status = (int)HttpStatusCode.InternalServerError,
				Type = "Internal server error has occurred"
			});

		var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();
		if (environment.IsDevelopment()) {
			problem!.Title = exception.Message;
			problem.Detail = exception.Unwrap();

			var sb = new StringBuilder();

			if (!string.IsNullOrEmpty(exception.Source)) {
				sb.Append(exception.Source);
				if (exception.TargetSite != null) {
					sb.Append("; ");
				}
			}

			if (exception.TargetSite != null) {
				sb
					.Append(exception.TargetSite.DeclaringType?.Name)
					.Append('.')
					.Append(exception.TargetSite.Name);
			}

			problem.Instance = sb.ToString();
		}

		context.Response.ContentType = MediaTypeNames.Application.Json;
		context.Response.StatusCode = problem!.Status.GetValueOrDefault((int)HttpStatusCode.InternalServerError);

		await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
	}
}