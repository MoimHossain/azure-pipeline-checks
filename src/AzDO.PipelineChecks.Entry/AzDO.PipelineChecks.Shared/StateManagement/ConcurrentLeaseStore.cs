

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
            var leaseAcquired = false;
            var startTime = DateTime.UtcNow;
            while (!leaseAcquired && DateTime.UtcNow - startTime < timeOut)
            {
                try
                {
                    await daprClient.TrySaveStateAsync<Lease>(Constants.Dapr.State.Leases, leaseKey, lease, etag: leaseKey);
                    leaseAcquired = true;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to acquire lease {LeaseName} - will retry if timeout has not occured.", leaseName);
                    await Task.Delay(pollingDelayInMilliseconds);
                }
            }


            if (!leaseAcquired)
            {
                logger.LogError("Failed to acquire lease {LeaseName} within {TimeOut}", leaseName, timeOut);
            }


            logger.LogInformation("Lease {LeaseName} acquired", leaseName);
            return new InternalDisposableForLeaseManagement(this, leaseName, leaseKey, leaseAcquired);
        }

        private async Task ReleaseLeaseAsync(string leaseName, string leaseKey, bool acquired)
        {
            if(acquired)
            {
                await daprClient.DeleteStateAsync(Constants.Dapr.State.Leases, leaseKey).ConfigureAwait(false);
                logger.LogInformation("Lease {LeaseName} released", leaseName);
            }
        }


        public class InternalDisposableForLeaseManagement : IDisposable
        {
            private readonly ConcurrentLeaseStore _leaseStore;
            private readonly string _leaseName;
            private readonly string _leaseKey;
            private readonly bool _aquired;

            public InternalDisposableForLeaseManagement(
                ConcurrentLeaseStore leaseStore, string leaseName, 
                string leaseKey, bool aquired)
            {
                _leaseStore = leaseStore;
                _leaseName = leaseName;
                _leaseKey = leaseKey;
                _aquired = aquired;
            }
            public void Dispose()
            {                
                _leaseStore.ReleaseLeaseAsync(_leaseName, _leaseKey, _aquired).Wait();
            }

            public bool Aquired => _aquired;
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


