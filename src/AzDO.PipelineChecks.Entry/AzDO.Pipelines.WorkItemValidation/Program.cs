

using AzDO.PipelineChecks.Shared;
using AzDO.PipelineChecks.Shared.Endpoints;
using AzDO.Pipelines.WorkItemValidation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

await builder.Services.RegisterSharedServicesAsync(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();

var apiGroup = app.MapGroup("api");


apiGroup.MapPost("/validate", ValidationEndpoint.Handler)
    .WithName("Process Pipeline Validation")
    .WithDisplayName("Process Pipeline Validation")
    .WithOpenApi();

var daprApiGroup = app.MapGroup("dapr");
var endpoint = new DaprSubscriptionEndpoint(Constants.Dapr.Sub.WorkItem, Constants.Dapr.Topic_RequestReceived);
daprApiGroup.MapGet("/subscribe", endpoint.Handler)
    .WithName("Dapr Subscribe")
    .WithDisplayName("Dapr Subscribe").WithOpenApi();


app.Run();

