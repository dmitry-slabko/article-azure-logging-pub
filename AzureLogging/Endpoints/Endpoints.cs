using System.Net;
using AzureLogging.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureLogging.Endpoints; 

/// <summary>
/// Sample endpoints and their registration
/// </summary>
public static class Endpoints {
	private const string BaseUrl = "/api/samples";

	/// <summary>
	/// Registers all endpoints
	/// </summary>
	/// <param name="app"><see cref="IEndpointRouteBuilder"/></param>
	/// <returns><see cref="IEndpointRouteBuilder"/></returns>
	public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder app) {
		app.MapGet(BaseUrl, GetAll)
			.Produces<IEnumerable<SampleData>>();

		app.MapGet(BaseUrl + "/{id:int}", GetOne)
			.Produces<SampleData>()
			.ProducesProblem((int)HttpStatusCode.NotFound);

		app.MapPost(BaseUrl, Create)
			.Produces<SampleData>()
			.ProducesProblem((int)HttpStatusCode.BadRequest);

		return app;
	}

	/// <summary>
	/// Get all samples
	/// </summary>
	/// <param name="repository"><see cref="SampleRepository"/></param>
	/// <returns><see cref="IEnumerable{T}"/> of <see cref="SampleData"/></returns>
	public static Task<IResult> GetAll([FromServices] SampleRepository repository) => 
		Task.FromResult(Results.Ok(repository.GetAllSamples()));

	/// <summary>
	/// Get requested sample
	/// </summary>
	/// <param name="repository"><see cref="SampleRepository"/></param>
	/// <param name="id">Sample ID</param>
	/// <returns><see cref="SampleData"/></returns>
	public static Task<IResult> GetOne([FromServices] SampleRepository repository, int id) {
		var sample = repository.GetSample(id);
		return Task.FromResult(sample is null ? Results.NotFound() : Results.Ok(sample));
	}

	/// <summary>
	/// Create a new sample
	/// </summary>
	/// <param name="repository"><see cref="SampleRepository"/></param>
	/// <param name="request"><see cref="SampleRequest"/></param>
	/// <returns>New <see cref="SampleData"/></returns>
	public static Task<IResult> Create([FromServices] SampleRepository repository, [FromBody] SampleRequest request) => 
		Task.FromResult(Results.Ok(repository.CreateSample(request.Name)));
}