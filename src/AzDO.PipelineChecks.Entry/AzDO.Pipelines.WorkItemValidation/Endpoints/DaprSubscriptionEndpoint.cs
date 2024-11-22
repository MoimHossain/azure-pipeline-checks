
using AzDO.PipelineChecks.Shared;

namespace AzDO.Pipelines.WorkItemValidation.Endpoints
{
    public class DaprSubscriptionEndpoint
    {
        public static async Task<object> Handler(
            HttpContext context,
            IntegrationService integrationService,
            ILogger<DaprSubscriptionEndpoint> logger,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("########################################### Received Dapr subscription request");

            await Task.CompletedTask;
            return new List<object>
            {
                new {
                    pubsubname = Constants.DaprComponents.PubSubEntrySubscription,
                    topic = Constants.DaprComponents.Topic,
                    route = "/api/process"
                }
            };

        }
    }
}
