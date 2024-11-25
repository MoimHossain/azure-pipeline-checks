

using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace AzDO.PipelineChecks.Shared
{
    public class IntegrationService(DaprClient daprClient, ILogger<IntegrationService> logger)
    {
        public async Task PublishCheckEntryEventAsync(HttpHeaderCollection payload, CancellationToken cancellationToken)
        {
            logger.LogInformation("Publishing CheckEntry event {payload}", payload);

            await daprClient.PublishEventAsync(
                Constants.Dapr.PubSub_Entry, 
                Constants.Dapr.Topic_RequestReceived, 
                payload, cancellationToken);
        }

        public async Task PublishValidationCompletedEventAsync(
            CheckKind checkKind, ValidationResult validationResult, 
            HttpHeaderCollection httpHeaderCollection,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Publishing ValidationCompleted event {BuildId} {JobId} {IsValid} {CheckKind} {ComputationKind} {StageName}", 
                validationResult.BuildId, validationResult.JobId, validationResult.IsValid, checkKind.ToString(), validationResult.CheckComputation, validationResult.StageName);

            var completedEvent = new ValidationCompletedEvent
            {
                CheckKind = checkKind,
                CheckComputation = validationResult.CheckComputation,
                IsValid = validationResult.IsValid,                
                Errors = validationResult.Errors,
                BuildId = validationResult.BuildId,
                CreationTime = validationResult.CreationTime,
                DefinitionId = validationResult.DefinitionId,
                JobId = validationResult.JobId,
                PlanId = validationResult.PlanId,
                CheckStageId = validationResult.CheckStageId,
                ProjectId = validationResult.ProjectId,
                StageId = validationResult.StageId,
                StageName = validationResult.StageName
            };

            await daprClient.PublishEventAsync(
                Constants.Dapr.PubSub_ValidationOutcome,
                Constants.Dapr.Topic_RequestEvaluated,
                new OutcomePackageDto(httpHeaderCollection, completedEvent), cancellationToken);
        }
    }
}
