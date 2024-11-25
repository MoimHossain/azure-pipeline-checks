

using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

const string partitionKey = "check-entry";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(services =>
{
    var jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };
    jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    return jsonSerializerOptions;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<TableClient>((serviceProvider) =>
{
    var tableClient = new TableClient(Environment.GetEnvironmentVariable("STORAGE_CONN"), "visualizations");
    return tableClient;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/visualize/{buildId}", (int buildId, [FromServices] TableClient tableClient) =>
{
    List<BuildInfo> collection = [];
    
    var filter = TableClient.CreateQueryFilter(
        $"PartitionKey eq {partitionKey} and RowKey ge {buildId.ToString()} and RowKey lt {(buildId + 1).ToString()}");

    Pageable<TableEntity> queryResults = tableClient.Query<TableEntity>(filter);
    
    
    foreach (var entity in queryResults)
    {
        if(entity != null )
        {
            var valueAsString = entity.GetString("Value");
            var buildInfo = JsonSerializer.Deserialize<BuildInfo>(valueAsString);
            if(buildInfo != null)
            {
                buildInfo.Timestamp = entity.Timestamp;

                collection.Add(buildInfo);
            }
        }
    }

    collection = collection.OrderBy(x => x.Timestamp).ToList();

    return Results.Ok(collection);

}).WithName("Get Visualization data").WithOpenApi();

app.Run();


public class BuildInfo
{
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    [JsonPropertyName("buildId")]
    public int BuildId { get; set; }

    [JsonPropertyName("stageId")]

    public string? StageId { get; set; }

    [JsonPropertyName("stageName")]
    public string? StageName { get; set; }

    [JsonPropertyName("jobId")]
    public string? JobId { get; set; }

    [JsonPropertyName("CheckKind")]
    public string? CheckKind { get; set; }

    [JsonPropertyName("CheckStatus")]
    public string? CheckStatus { get; set; }
}