

using Dapr.Client;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared.StateManagement
{
    public class ConcurrentLeaseStore(DaprClient daprClient, ILogger<ConcurrentLeaseStore> logger)
    {
        public async Task<InternalDisposableForLeaseManagement> AquireLeaseAsync(
            string leaseName, TimeSpan timeOut, CancellationToken cancellationToken, 
            int pollingDelayInMilliseconds = 1000) 
        {
            var leaseKey = $"leases-{leaseName}";
            var lease = new Lease { LeaseName = leaseName, LeaseOwner = Environment.MachineName };
            var leaseOptions = new StateOptions { Consistency = ConsistencyMode.Strong };
            var leaseAcquired = false;
            var startTime = DateTime.UtcNow;
            while (!leaseAcquired && DateTime.UtcNow - startTime < timeOut)
            {
                try
                {
                    await daprClient.SaveStateAsync(Constants.Dapr.State.Leases, leaseKey, lease, leaseOptions, 
                        cancellationToken: cancellationToken);                    

                    leaseAcquired = true;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to acquire lease {LeaseName}", leaseName);
                    await Task.Delay(pollingDelayInMilliseconds);
                }
            }


            if (!leaseAcquired)
            {
                logger.LogError("Failed to acquire lease {LeaseName} within {TimeOut}", leaseName, timeOut);
                throw new TimeoutException($"Failed to acquire lease {leaseName} within {timeOut}");
            }


            logger.LogInformation("Lease {LeaseName} acquired", leaseName);
            return new InternalDisposableForLeaseManagement(this, leaseName);
        }

        public async Task ReleaseLeaseAsync(string leaseName)
        {
            var leaseKey = $"leases-{leaseName}";
            await daprClient.DeleteStateAsync(Constants.Dapr.State.Leases, leaseKey).ConfigureAwait(false);
            logger.LogInformation("Lease {LeaseName} released", leaseName);
        }


        public class InternalDisposableForLeaseManagement : IDisposable
        {
            private readonly ConcurrentLeaseStore _leaseStore;
            private readonly string _leaseName;
            public InternalDisposableForLeaseManagement(ConcurrentLeaseStore leaseStore, string leaseName)
            {
                _leaseStore = leaseStore;
                _leaseName = leaseName;
            }
            public void Dispose()
            {                
                _leaseStore.ReleaseLeaseAsync(_leaseName).Wait();
            }
        }

    }

    public class Lease
    {
        [JsonPropertyName("leaseName")]
        public string? LeaseName { get; set; }

        [JsonPropertyName("leaseOwner")]
        public string? LeaseOwner { get; set; }
    }
}


//var succes = await daprClient.TrySaveStateAsync<CustomState>(stateStoreName, "key", payload, "MOIMHOSSAINX");