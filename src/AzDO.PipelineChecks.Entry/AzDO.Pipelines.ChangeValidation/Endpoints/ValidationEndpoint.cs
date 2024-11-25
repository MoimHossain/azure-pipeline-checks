

using AzDO.PipelineChecks.Shared;
using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.PipelineServices;
using AzDO.PipelineChecks.Shared.StateManagement;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Microsoft.AspNetCore.Mvc;

namespace AzDO.Pipelines.ChangeValidation.Endpoints
{
    public class ValidationEndpoint
    {
        private static async Task ProcessValidationCoreAsync(HttpHeaderCollection headers, ILogger<ValidationEndpoint> logger, 
            IntegrationService integrationService, StateStoreService stateStoreService, 
            PipelineService pipelineService, ValidationArguments validationArguments, CancellationToken cancellationToken)
        {
            var validationResult = await stateStoreService.GetChangeValidationResultAsync(validationArguments, cancellationToken);
            if (validationResult == null)
            {
                // TODO - Implement validation logic here                    
                var delay = new Random().Next(3, 5);
                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

                validationResult = ChangeValidationResult.CreateFrom(validationArguments, isValid: true);
                var validationResultInString = validationResult.IsValid ? "PASSED" : "FAILED";

                await stateStoreService.SaveChangeValidationResultAsync(validationResult, validationArguments, cancellationToken);
                await pipelineService.ReportTaskProgressAsync($"Change validation completed ({validationResultInString})", headers, cancellationToken);
            }
            else
            {
                var validationResultInString = validationResult.IsValid ? "PASSED" : "FAILED";
                logger.LogInformation("Validation result already exists for {BuildId} {Result}", validationArguments.BuildId, validationResultInString);
                await pipelineService.ReportTaskProgressAsync($"Change validation ({validationResultInString}) computed before. (skipping)", headers, cancellationToken);
            }

            await integrationService.PublishValidationCompletedEventAsync(CheckKind.Change, validationResult, headers, cancellationToken);
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

                var leaseName = $"ChangeValidation-{validationArguments.BuildId}";

                using var lease = await concurrentLeaseStore.AquireLeaseAsync(leaseName, TimeSpan.FromSeconds(60), cancellationToken);

                await ProcessValidationCoreAsync(envelope.Data, logger, integrationService, stateStoreService,
                    pipelineService, validationArguments, cancellationToken);
            }
            else
            {
                logger.LogWarning("Received empty or invalid validation request");
            }
            return new { Ok = true };
        }
    }
}
