

using AzDO.PipelineChecks.Shared;
using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.PipelineServices;
using AzDO.PipelineChecks.Shared.StateManagement;
using AzDO.PipelineChecks.Shared.Utils;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Microsoft.AspNetCore.Mvc;

namespace AzDO.PipelineChecks.Entry.Endpoints
{
    public class CheckEntryEndpoint
    {
        public static async Task<IResult> Handler(
            HttpContext context,
            [FromServices] IntegrationService integrationService,
            [FromServices] HttpHeaderTraceClient httpHeaderTraceClient,
            [FromServices] PipelineService pipelineService,            
            [FromServices] ILogger<CheckEntryEndpoint> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                httpHeaderTraceClient.TraceHeaders(context);

                var payload = context.Request.Headers.From();

                await integrationService.PublishCheckEntryEventAsync(payload, cancellationToken);

                await pipelineService.ReportTaskBegingAsync(payload, cancellationToken);

                var validationArgs = ValidationArguments.ReadFromRequestHeader(payload);

                return Results.Accepted("Pipeline Check request received successfully.");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error processing request");
                return Results.Problem(e.Message); // fix this later
            }
        }
    }
}