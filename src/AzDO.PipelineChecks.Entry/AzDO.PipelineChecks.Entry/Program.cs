

using AzDO.PipelineChecks.Entry.Endpoints;
using AzDO.PipelineChecks.Shared;

var builder = WebApplication.CreateBuilder(args);

await builder.Services.RegisterSharedServicesAsync(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();

var apiGroup = app.MapGroup("api");
apiGroup.MapPost("/begin-validation", CheckEntryEndpoint.Handler)
    .WithName("Begin Pipeline Validation")
    .WithDisplayName("Begin Pipeline Validation").WithOpenApi();

apiGroup.MapPost("/process", LisenerEndpoint.Handler)
    .WithName("Process Pipeline Validation")
    .WithDisplayName("Process Pipeline Validation")    
    .WithOpenApi();

var daprApiGroup = app.MapGroup("dapr");
daprApiGroup.MapGet("/subscribe", DaprSubscriptionEndpoint.Handler)
    .WithName("Dapr Subscribe")
    .WithDisplayName("Dapr Subscribe").WithOpenApi();


app.Run();

