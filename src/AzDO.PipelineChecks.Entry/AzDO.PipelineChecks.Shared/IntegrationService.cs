﻿

using AzDO.PipelineChecks.Shared.ValidationDto;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace AzDO.PipelineChecks.Shared
{
    public class IntegrationService(DaprClient daprClient, ILogger<IntegrationService> logger)
    {
        public async Task PublishCheckEntryEventAsync(object payload, CancellationToken cancellationToken)
        {
            logger.LogInformation("Publishing CheckEntry event {payload}", payload);

            await daprClient.PublishEventAsync(
                Constants.Dapr.PubSub_Entry, 
                Constants.Dapr.Topic_RequestReceived, 
                payload, cancellationToken);
        }

        public async Task PublishValidationCompletedEventAsync(ValidationResult validationResult, CancellationToken cancellationToken)
        {
            logger.LogInformation("Publishing ValidationCompleted event {BuildId}", validationResult.BuildId);

            var completedEvent = new ValidationCompletedEvent
            {
                CheckKind = CheckKind.WorkItem,
                BuildId = validationResult.BuildId,
                Errors = validationResult.Errors,
                IsValid = validationResult.IsValid,
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
                completedEvent, cancellationToken);
        }
    }
}
