
using AzDO.PipelineChecks.Shared;

namespace AzDO.Pipelines.WorkItemValidation.Endpoints
{
    public class DaprSubscriptionEndpoint
    {
        public static async Task<object> Handler(ILogger<DaprSubscriptionEndpoint> logger)
        {
            logger.LogInformation("### Received Dapr subscription request ###");

            await Task.CompletedTask;

            return new List<object>
            {
                new {
                    pubsubname = Constants.DaprComponents.PubSubEntrySubscription,
                    topic = Constants.DaprComponents.Topic,
                    route = "/api/validate"
                }
            };
        }
    }
}
