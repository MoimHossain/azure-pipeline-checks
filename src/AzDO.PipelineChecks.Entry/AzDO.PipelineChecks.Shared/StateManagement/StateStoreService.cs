﻿

using AzDO.PipelineChecks.Shared.ValidationDto;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace AzDO.PipelineChecks.Shared.StateManagement
{
    public class StateStoreService(DaprClient daprClient, ILogger<StateStoreService> logger)
    {
        public async Task SaveWorkItemValidationResultAsync(
            WorkItemValidationResult workItemValidationResult,
            ValidationArguments validationArguments,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Saving (overwrite) work item validation result: {BuildId}", workItemValidationResult.BuildId);

            var rowKey = workItemValidationResult.GetRowKey();

            await daprClient.SaveStateAsync(
                Constants.Dapr.State.WorkItemValidations,
                rowKey,
                workItemValidationResult,
                new StateOptions { Consistency = ConsistencyMode.Strong },
                cancellationToken: cancellationToken);
        }

        public async Task<WorkItemValidationResult?> GetWorkItemValidationResultAsync(
            ValidationArguments validationArguments, CancellationToken cancellationToken)
        {
            WorkItemValidationResult? result = default;
            try
            {
                logger.LogInformation("Getting work item validation result for {BuildId}", validationArguments.BuildId);
                var rowKey = WorkItemValidationResult.CreateFrom(validationArguments).GetRowKey();
                result = await daprClient.GetStateAsync<WorkItemValidationResult>(
                                Constants.Dapr.State.WorkItemValidations,
                                rowKey,
                                cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting work item validation result");
            }
            return result;
        }
    }
}