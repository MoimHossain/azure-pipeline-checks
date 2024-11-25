

using AzDO.PipelineChecks.Shared;
using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.PipelineServices;
using AzDO.PipelineChecks.Shared.StateManagement;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Microsoft.AspNetCore.Mvc;

namespace AzDO.Pipelines.WorkItemValidation.Endpoints
{
    public class ValidationEndpoint
    {
        private static async Task ProcessValidateCoreAsync(
            HttpHeaderCollection headers, ILogger<ValidationEndpoint> logger, 
            IntegrationService integrationService, StateStoreService stateStoreService, 
            PipelineService pipelineService, ValidationArguments validationArguments, CancellationToken cancellationToken)
        {
            var validationResult = await stateStoreService.GetWorkItemValidationResultAsync(validationArguments, cancellationToken);
            if (validationResult == null)
            {
                // TODO - Implement validation logic here                                        
                var delay = new Random().Next(3, 5);
                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

                validationResult = WorkItemValidationResult.CreateFrom(validationArguments, isValid: true);
                var validationResultInString = validationResult.IsValid ? "PASSED" : "FAILED";

                await stateStoreService.SaveWorkItemValidationResultAsync(validationResult, validationArguments, cancellationToken);
                await pipelineService.ReportTaskProgressAsync($"Work item validation completed ({validationResultInString})", headers, cancellationToken);
            }
            else
            {
                var validationResultInString = validationResult.IsValid ? "PASSED" : "FAILED";
                logger.LogInformation("Validation result already exists for {BuildId} {Result}", validationArguments.BuildId, validationResultInString);
                await pipelineService.ReportTaskProgressAsync($"WorkItem validation ({validationResultInString}) computed before. (Skipping)", headers, cancellationToken);
            }

            await integrationService.PublishValidationCompletedEventAsync(CheckKind.WorkItem, validationResult, headers, cancellationToken);
        }

        public static async Task<object> Handler(
            [FromBody] Envelope<HttpHeaderCollection> envelope,
            [FromServices] ILogger<ValidationEndpoint> logger,
            [FromServices] IntegrationService integrationService,
            [FromServices] StateStoreService stateStoreService,
            [FromServices] PipelineService pipelineService,
            [FromServices] ConcurrentLeaseStore concurrentLeaseStore,
            CancellationToken cancellationToken)
        {
            if (envelope != null && envelope.Data != null)
            {
                logger.LogInformation("Received validation request: {Headers}", envelope.Data.Headers.Count);

                var validationArguments = ValidationArguments.ReadFromRequestHeader(envelope.Data);

                var leaseName = $"WorkItem-{validationArguments.BuildId}";
                
                using var lease = await concurrentLeaseStore.AquireLeaseAsync(leaseName, TimeSpan.FromSeconds(60), cancellationToken);

                await ProcessValidateCoreAsync(envelope.Data, logger, integrationService, stateStoreService, pipelineService, validationArguments, cancellationToken);
            }
            else
            {
                logger.LogWarning("Received empty or invalid validation request");
            }
            return new { Ok = true };
        }
    }
}
