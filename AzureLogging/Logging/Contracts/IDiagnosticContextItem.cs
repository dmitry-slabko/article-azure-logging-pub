using Serilog;

namespace AzureLogging.Logging.Contracts; 

/// <summary>
/// An interface to provide <see cref="IDiagnosticContext"/> data items during HTTP request logging
/// </summary>
public interface IDiagnosticContextItem {
	/// <summary>
	/// <see cref="IDiagnosticContext"/> property name; see <see cref="IDiagnosticContext.Set"/> method
	/// </summary>
	string Key { get; }

	/// <summary>
	/// Gets <see cref="IDiagnosticContext"/> property value; see <see cref="IDiagnosticContext.Set"/> method
	/// </summary>
	object GetData(HttpContext context);
}