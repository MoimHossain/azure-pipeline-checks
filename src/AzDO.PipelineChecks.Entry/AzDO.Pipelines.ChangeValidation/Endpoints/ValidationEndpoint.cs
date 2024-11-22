

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
        public static async Task<object> Handler(
            [FromBody] Envelope<HttpHeaderCollection> envelope,
            [FromServices] ILogger<ValidationEndpoint> logger,
            [FromServices] IntegrationService integrationService,
            [FromServices] StateStoreService stateStoreService,
            [FromServices] PipelineService pipelineService,
            CancellationToken cancellationToken)
        {
            if (envelope != null && envelope.Data != null)
            {
                logger.LogInformation("Received validation request: {Headers}", envelope.Data.Headers.Count);

                var validationArguments = ValidationArguments.ReadFromRequestHeader(envelope.Data);

                var validationResult = await stateStoreService.GetChangeValidationResultAsync(validationArguments, cancellationToken);
                if (validationResult == null)
                {
                    // TODO - Implement validation logic here

                    validationResult = ChangeValidationResult.CreateFrom(validationArguments, isValid: true);

                    await stateStoreService.SaveChangeValidationResultAsync(validationResult, validationArguments, cancellationToken);
                    await pipelineService.ReportTaskProgressAsync("Change validation completed", envelope.Data, cancellationToken);
                }
                else
                {
                    logger.LogInformation("Validation result already exists for {BuildId}", validationArguments.BuildId);
                    await pipelineService.ReportTaskProgressAsync("Change validation (skipping)", envelope.Data, cancellationToken);
                }

                await integrationService.PublishValidationCompletedEventAsync( CheckKind.Change, validationResult, cancellationToken);
            }
            else
            {
                logger.LogWarning("Received empty or invalid validation request");
            }
            return new { Ok = true };
        }
    }
}
