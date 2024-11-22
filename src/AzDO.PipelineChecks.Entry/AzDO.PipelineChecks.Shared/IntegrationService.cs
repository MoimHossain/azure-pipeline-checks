

using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace AzDO.PipelineChecks.Shared
{
    public class IntegrationService(DaprClient daprClient, ILogger<IntegrationService> logger)
    {
        public async Task InvokeAsync(
            string targetName, object payload, CancellationToken cancellationToken)
        {
            logger.LogInformation("Invoking integration {targetName}", targetName);


            await daprClient.PublishEventAsync("azdo-pipeline-check-entry", "entryrequests", payload);
        }
    }
}
