namespace AzureLogging.Models; 

/// <summary>
/// Sample data class
/// </summary>
public sealed class SampleData {
	/// <summary>
	/// Sample ID
	/// </summary>
	public int Id { get; init; }

	/// <summary>
	/// Sample name
	/// </summary>
	public string Name { get; init; } = default!;
}