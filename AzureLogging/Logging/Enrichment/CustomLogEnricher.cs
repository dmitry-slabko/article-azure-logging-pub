using Serilog.Core;
using Serilog.Events;

namespace AzureLogging.Logging.Enrichment; 

/// <summary>
/// Implements <see cref="ILogEventEnricher"/> to enrich logs with custom information
/// </summary>
public sealed class CustomLogEnricher : ILogEventEnricher {
	private readonly Dictionary<string, List<string>> properties = new();

	/// <summary>
	/// Adds custom data to enrich log events
	/// </summary>
	/// <param name="property">Log property name</param>
	/// <param name="value">Log property value</param>
	/// <returns>Itself for call chaining</returns>
	public CustomLogEnricher With(string property, string value) {
		if (string.IsNullOrWhiteSpace(value)) {
			return this;
		}

		if (properties.TryGetValue(property, out var values)) {
			values.Add(value);
		} else {
			properties.Add(property, new () { value });
		}

		return this;
	}

	/// <inheritdoc cref="ILogEventEnricher"/>
	void ILogEventEnricher.Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
		foreach (var item in properties) {
			LogEventPropertyValue value = item.Value.Count == 1
				? new ScalarValue(item.Value[0])
				: new SequenceValue(item.Value.Select(x => new ScalarValue(x)));
			var property = new LogEventProperty(item.Key, value);

			logEvent.AddOrUpdateProperty(property);
		}
	}
}