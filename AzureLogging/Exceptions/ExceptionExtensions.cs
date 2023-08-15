using System.Text;

namespace AzureLogging.Exceptions; 

/// <summary>
/// Helper class to retrieve all inner and aggregate exceptions messages and stack traces from an exception
/// </summary>
internal static class ExceptionExtensions {
	/// <summary>
	/// Returns messages and stack traces from complete InnerException tree
	/// </summary>
	/// <param name="ex">Exception instance</param>
	/// <returns>All stack traces from the exception</returns>
	public static string Unwrap(this Exception ex) => ex.UnwrapTopLevelWithAction(Log);

	/// <summary>
	/// Returns stack traces from complete InnerException tree
	/// </summary>
	/// <param name="ex">Exception instance</param>
	/// <returns>All stack traces from the exception</returns>
	public static string UnwrapStackTraces(this Exception ex) => ex.UnwrapTopLevelWithAction(LogStackTrace);

	/// <summary>
	/// Returns messages from complete InnerException tree
	/// </summary>
	/// <param name="ex">Exception instance</param>
	/// <returns>All messages from the exception</returns>
	public static string UnwrapMessages(this Exception ex) => ex.UnwrapTopLevelWithAction(LogMessage);

	private static string UnwrapTopLevelWithAction(this Exception ex, Action<StringBuilder, Exception, bool> loggingAction) {
		var sb = new StringBuilder();

		loggingAction(sb, ex, false);

		if (ex is AggregateException aggregate) {
			foreach (var item in aggregate.Flatten().InnerExceptions) {
				item.UnwrapWithAction(exception => loggingAction(sb, exception, true));
			}
		} else {
			var item = ex.InnerException;
			if (item is not null) {
				item.UnwrapWithAction(exception => loggingAction(sb, exception, true));
			}
		}

		return sb.ToString();
	}

	private static void UnwrapWithAction(this Exception ex, Action<Exception> loggingAction) {
		loggingAction(ex);

		if (ex is AggregateException aggregate) {
			foreach (var item in aggregate.Flatten().InnerExceptions) {
				item.UnwrapWithAction(loggingAction);
			}
		} else {
			var item = ex.InnerException;
			if (item is not null) {
				item.UnwrapWithAction(loggingAction);
			}
		}
	}

	private static void Log(StringBuilder builder, Exception exception, bool appendLine) {
		if (appendLine) {
			builder.AppendLine();
		}

		builder
			.Append("Exception of type ")
			.Append(exception.GetType().Name)
			.Append(" with message: ")
			.AppendLine(exception.Message)
			.Append("was thrown at: ")
			.Append(exception.StackTrace);
	}

	private static void LogStackTrace(StringBuilder builder, Exception exception, bool appendLine) {
		if (appendLine) {
			builder.AppendLine();
		}

		builder
			.Append("Exception of type ")
			.Append(exception.GetType().Name)
			.Append(" was thrown at: ")
			.Append(exception.StackTrace);
	}

	private static void LogMessage(StringBuilder builder, Exception exception, bool appendLine) {
		if (appendLine) {
			builder.AppendLine();
		}

		builder
			.Append("Exception of type ")
			.Append(exception.GetType().Name)
			.Append(" was thrown: ")
			.Append(exception.Message);
	}
}