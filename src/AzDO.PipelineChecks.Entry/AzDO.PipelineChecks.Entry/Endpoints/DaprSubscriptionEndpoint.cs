

using AzDO.PipelineChecks.Shared;

namespace AzDO.PipelineChecks.Entry.Endpoints
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
                    pubsubname = "azdo-pipeline-check-entry-listener",
                    topic = "entryrequests",
                    route = "/api/process"
                }
            };
 
        }
    }
}
