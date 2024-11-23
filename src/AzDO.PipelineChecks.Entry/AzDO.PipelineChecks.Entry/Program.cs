

using AzDO.PipelineChecks.Entry.Endpoints;
using AzDO.PipelineChecks.Shared;
using AzDO.PipelineChecks.Shared.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var app = await builder.BuidAndConfigureAppAsync(Constants.MicroServices.Entry);

var apiGroup = app.MapGroup("api");
apiGroup.MapPost("/begin-validation", CheckEntryEndpoint.Handler)
    .WithName("Begin Pipeline Validation")
    .WithDisplayName("Begin Pipeline Validation").WithOpenApi();

apiGroup.MapPost("/process-outcome", OutcomeEndpoint.Handler)
    .WithName("Process Pipeline Validation outcome")
    .WithDisplayName("Process Pipeline Validation outcome")
    .WithOpenApi();

var daprApiGroup = app.MapGroup("dapr");
var endpoint = new DaprSubscriptionEndpoint(
    Constants.Dapr.Sub.OutcomeAggragator, Constants.Dapr.Topic_RequestEvaluated, route: "/api/process-outcome");
daprApiGroup.MapGet("/subscribe", endpoint.Handler)
    .WithName("Dapr Subscribe")
    .WithDisplayName("Dapr Subscribe").WithOpenApi();

app.Run();

