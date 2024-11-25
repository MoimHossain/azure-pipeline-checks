

using AzDO.PipelineChecks.Shared.ValidationDto;
using AzDO.PipelineChecks.Shared.Visualizations;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace AzDO.PipelineChecks.Shared.StateManagement
{
    public class VisualizationStore(DaprClient daprClient, ILogger<VisualizationStore> logger)
    {
        public async Task RecordAsync(
            int buildId, Guid stageId, Guid jobId, CheckKind kind,
            CheckResultCompuationKind checkResultCompuationKind,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Recording visualization for {BuildId} {StageId} {JobId} {CheckKind} {ComputationKind}",
                buildId, stageId, jobId, kind, checkResultCompuationKind);
            var rowKey = VisualizationDto.GetRowKey(buildId, stageId, jobId, kind, checkResultCompuationKind);
            var entity = new VisualizationDto
            {
                BuildId = buildId,
                StageID = stageId,
                JobId = jobId,
                CheckKind = kind.ToString(),
                CompuationKind = checkResultCompuationKind.ToString()
            };

            await daprClient.SaveStateAsync(
                Constants.Dapr.State.Visualizations,
                rowKey,
                entity,
                new StateOptions { Consistency = ConsistencyMode.Strong },
                cancellationToken: cancellationToken);
        }
    }
}
