

using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace AzDO.PipelineChecks.Shared
{
    public class IntegrationService(DaprClient daprClient, ILogger<IntegrationService> logger)
    {
        public async Task PublishEventAsync(object payload, CancellationToken cancellationToken)
        {
            logger.LogInformation("Invoking integration with payload: {payload}", payload);

            await daprClient.PublishEventAsync(
                Constants.Dapr.PubSub_Entry, 
                Constants.Dapr.Topic, 
                payload, cancellationToken);
        }
    }
}
