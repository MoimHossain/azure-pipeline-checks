

using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.Payloads;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AzDO.Pipelines.WorkItemValidation.Endpoints
{
    public class LisenerEndpoint
    {
        public static async Task<object> Handler(
            [FromBody] Envelope<RequestPayload> envelope,

            ILogger<LisenerEndpoint> logger,
            CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            logger.LogInformation("Received request with body: {body}", JsonSerializer.Serialize(envelope));

            return new { Ok = true };
        }
    }
}
