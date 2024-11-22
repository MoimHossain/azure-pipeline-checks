

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


app.Run();

