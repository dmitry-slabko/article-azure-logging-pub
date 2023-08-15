using Bogus;
using ILogger = Serilog.ILogger;

namespace AzureLogging.Models; 

/// <summary>
/// Primitive repository to get and create samples
/// </summary>
public sealed class SampleRepository {
	private readonly Faker<SampleData> fakeSample = new Faker<SampleData>()
		.StrictMode(true)
		.UseSeed(123456)
		.RuleFor(x => x.Id, faker => faker.IndexFaker)
		.RuleFor(x => x.Name, faker => faker.Company.CompanyName(0));

	private readonly List<SampleData> samples = new();
	private readonly ILogger logger;

	/// <summary>
	/// Constructor that prefills fake samples
	/// </summary>
	public SampleRepository(ILogger logger) {
		this.logger = logger;
		samples.AddRange(Enumerable.Range(0, 12).Select(x => fakeSample.Generate()));
	}

	/// <summary>
	/// Returns all samples
	/// </summary>
	public IEnumerable<SampleData> GetAllSamples() => samples.AsEnumerable();

	/// <summary>
	/// Returns a sample matching the specified id
	/// </summary>
	/// <param name="id">Sample id</param>
	/// <returns>Matched <see cref="SampleData"/> or null</returns>
	public SampleData? GetSample(int id) => samples.SingleOrDefault(x => x.Id == id);

	/// <summary>
	/// Creates a new sample with given name and adds it to all samples
	/// </summary>
	/// <param name="name">New sample name</param>
	/// <returns>New <see cref="SampleData"/></returns>
	public SampleData CreateSample(string name) {
		var sample = new SampleData {
			Id = fakeSample.Generate().Id,
			Name = name
		};
		samples.Add(sample);

		return sample;
	}
}