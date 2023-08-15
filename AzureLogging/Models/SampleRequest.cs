namespace AzureLogging.Models; 

/// <summary>
/// Sample request
/// </summary>
/// <param name="Name">Name</param>
/// <param name="CreatedBy">Created by id</param>
public record SampleRequest(string Name, Guid CreatedBy);