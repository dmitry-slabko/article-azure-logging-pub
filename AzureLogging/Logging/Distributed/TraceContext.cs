using System.Text;

namespace AzureLogging.Logging.Distributed; 

/// <summary>
/// This is a simple helper class to build a 'traceparent' header value.
/// See https://w3c.github.io/trace-context/#traceparent-header-field-values for more details.
/// </summary>
public sealed class TraceContext {
	private const string Version = "00";

	private const int FlagSampled = 1;
	private const int FlagRandom = 2;

	private const int ParentIdLength = 16;
	private const int CorrelationIdLength = 32;

	private static readonly Random Rnd = new();

	private string? traceParentValue;

	/// <summary>
	/// Default constructor that initializes all values randomly.
	/// </summary>
	public TraceContext() {
		var bytes = new byte[8];
		Rnd.NextBytes(bytes);
		ParentId = string.Concat(bytes.Select(b => b.ToString("x2")));
		CorrelationId = Guid.NewGuid().ToString("N");
	}

	/// <summary>
	/// Constructor that builds up a trace context from provided values;
	/// the values are validated to match the requirements.
	/// </summary>
	/// <param name="parentId">Trace context parent id.</param>
	/// <param name="correlationId">Trace context correlation id.</param>
	/// <exception cref="ArgumentException">Thrown if either <paramref name="parentId"/> or <paramref name="correlationId"/> are invalid.</exception>
	public TraceContext(string parentId, string correlationId) {
		if (parentId.Length != ParentIdLength || !IsHexadecimal(parentId)) {
			throw new ArgumentException("Must contain a 16 hexadecimal lowercase characters string", nameof(parentId));
		}

		if (correlationId.Length != CorrelationIdLength || !IsHexadecimal(correlationId)) {
			throw new ArgumentException("Must contain a 32 hexadecimal lowercase characters string", nameof(correlationId));
		}

		ParentId = parentId;
		CorrelationId = correlationId;
	}

	/// <summary>
	/// This flag should normally be false
	/// </summary>
	public bool Sampled { get; set; } = false;

	/// <summary>
	/// Randomly generated parent id; must contain 16 hexadecimal characters
	/// </summary>
	public string ParentId { get; }

	/// <summary>
	/// Randomly generated correlation id; must contain 32 hexadecimal characters
	/// </summary>
	public string CorrelationId { get; }

	/// <inheritdoc />
	public override string ToString() {
		if (traceParentValue is null) {
			var sb = new StringBuilder(Version, 55)
				.Append("-")
				.Append(CorrelationId)
				.Append("-")
				.Append(ParentId)
				.Append("-")
				.Append((FlagRandom | (Sampled ? FlagSampled : 0)).ToString("x2"));

			traceParentValue = sb.ToString();
		}

		return traceParentValue;
	}

	private static bool IsHexadecimal(ReadOnlySpan<char> span) {
		foreach (char c in span) {
			if (c is (< '0' or > '9') and (< 'a' or > 'f')) {
				return false;
			}
		}

		return true;
	}
}