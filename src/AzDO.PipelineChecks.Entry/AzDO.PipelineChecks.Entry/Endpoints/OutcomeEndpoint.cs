
using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Microsoft.AspNetCore.Mvc;

namespace AzDO.PipelineChecks.Entry.Endpoints
{
    public class OutcomeEndpoint
    {
        public static async Task<object> Handler(
            [FromBody] Envelope<ValidationCompletedEvent> envelope,
            ILogger<OutcomeEndpoint> logger,
            CancellationToken cancellationToken)
        {
            if (envelope != null && envelope.Data != null)
            {
                logger.LogInformation("Received outcome response: {payload}", envelope.Data);

                
                await Task.CompletedTask;
            }
            else
            {
                logger.LogWarning("Received empty or invalid validation request");
            }
            return new { Ok = true };
        }
    }
}
