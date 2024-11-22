

using Microsoft.Extensions.Logging;

namespace AzDO.PipelineChecks.Shared.Endpoints
{
    public class DaprSubscriptionEndpoint(
        string pubsubname, 
        string topic, 
        string route= "/api/validate")
    {
        public async Task<object> Handler(ILogger<DaprSubscriptionEndpoint> logger)
        {
            logger.LogInformation("### Received Dapr subscription request ###");

            await Task.CompletedTask;

            return new List<object>
            {
                new {
                    pubsubname,
                    topic,
                    route
                }
            };
        }
    }
}
