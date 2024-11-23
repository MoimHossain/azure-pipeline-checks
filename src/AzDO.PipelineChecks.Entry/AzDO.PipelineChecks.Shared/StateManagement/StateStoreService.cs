

using AzDO.PipelineChecks.Shared.ValidationDto;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace AzDO.PipelineChecks.Shared.StateManagement
{
    public class StateStoreService(DaprClient daprClient, ILogger<StateStoreService> logger)
    {
        public async Task<OutcomeDto?> GetOutcomeAsync(
            int definitionId, int buildId, CancellationToken cancellationToken)
        {
            OutcomeDto? outcome = default;
            try
            {
                logger.LogInformation("Getting outcome for {DefinitionId}-{BuildId}", definitionId, buildId);

                var rowKey = $"{definitionId}-{buildId}";
                outcome = await daprClient.GetStateAsync<OutcomeDto>(
                                Constants.Dapr.State.CheckOutcomes,
                                rowKey,
                                cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting outcome");
            }
            return outcome;
        }

        public async Task SaveOutcomeAsync(
            OutcomeDto outcome,            
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Saving (overwrite) outcome for {DefinitionId}-{BuildId}", outcome.DefinitionId, outcome.BuildId);

            var rowKey = $"{outcome.DefinitionId}-{outcome.BuildId}";

            await daprClient.SaveStateAsync(
                Constants.Dapr.State.CheckOutcomes,
                rowKey,
                outcome,
                new StateOptions { Consistency = ConsistencyMode.Strong },
                cancellationToken: cancellationToken);
        }

        public async Task SaveWorkItemValidationResultAsync(
            WorkItemValidationResult workItemValidationResult,
            ValidationArguments validationArguments,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Saving (overwrite) work item validation result: {BuildId}, Validation Passed = {isValid}", workItemValidationResult.BuildId, workItemValidationResult.IsValid);

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


        public async Task SaveChangeValidationResultAsync(
            ChangeValidationResult changeValidationResult,
            ValidationArguments validationArguments,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Saving (overwrite) Change validation result: {BuildId}, Validation Passed = {isValid}", changeValidationResult.BuildId, changeValidationResult.IsValid);

            var rowKey = changeValidationResult.GetRowKey();

            await daprClient.SaveStateAsync(
                Constants.Dapr.State.ChangeValidations,
                rowKey,
                changeValidationResult,
                new StateOptions { Consistency = ConsistencyMode.Strong },
                cancellationToken: cancellationToken);
        }

        public async Task<ChangeValidationResult?> GetChangeValidationResultAsync(
            ValidationArguments validationArguments, CancellationToken cancellationToken)
        {
            ChangeValidationResult? result = default;
            try
            {
                logger.LogInformation("Getting Change validation result for {BuildId}", validationArguments.BuildId);
                var rowKey = ChangeValidationResult.CreateFrom(validationArguments).GetRowKey();
                result = await daprClient.GetStateAsync<ChangeValidationResult>(
                                Constants.Dapr.State.ChangeValidations,
                                rowKey,
                                cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting Change validation result");
            }
            return result;
        }


        public async Task SetNotificationSentFlagAsync(            
            ValidationArguments validationArguments,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Setting notification sent flag for Build Id {BuildId} & Plant Id {PlanId}", validationArguments.BuildId, validationArguments.PlanId);

            var rowKey = $"{validationArguments.BuildId}-{validationArguments.PlanId}";

            await daprClient.SaveStateAsync(
                Constants.Dapr.State.CheckOutcomes,
                rowKey,
                new NotificationFlagDto { Notified = true },
                new StateOptions { Consistency = ConsistencyMode.Strong },
                cancellationToken: cancellationToken);
        }

        public async Task<bool> GetNotificationSentFlagAsync(
                ValidationArguments validationArguments, CancellationToken cancellationToken)
        {   
            try
            {
                logger.LogInformation("Getting notification sent flag for Build Id {BuildId} & Plant Id {PlanId}", validationArguments.BuildId, validationArguments.PlanId);

                var rowKey = $"{validationArguments.BuildId}-{validationArguments.PlanId}";
                var dto = await daprClient.GetStateAsync<NotificationFlagDto>(
                                Constants.Dapr.State.CheckOutcomes,
                                rowKey,
                                cancellationToken: cancellationToken);
                return dto != null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting Change validation result");
            }
            return false;
        }
    }
}
