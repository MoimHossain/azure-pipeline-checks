

using AzDO.PipelineChecks.Shared;
using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.StateManagement;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Microsoft.AspNetCore.Mvc;

namespace AzDO.Pipelines.WorkItemValidation.Endpoints
{
    public class ValidationEndpoint
    {
        public static async Task<object> Handler(
            [FromBody] Envelope<HttpHeaderCollection> envelope,
            [FromServices] ILogger<ValidationEndpoint> logger,
            [FromServices] IntegrationService integrationService,
            [FromServices] StateStoreService stateStoreService,
            CancellationToken cancellationToken)
        {
            if (envelope != null && envelope.Data != null)
            {
                logger.LogInformation("Received validation request: {Headers}", envelope.Data.Headers.Count);

                var validationArguments = ValidationArguments.ReadFromRequestHeader(envelope.Data);

                var validationResult = await stateStoreService.GetWorkItemValidationResultAsync(validationArguments, cancellationToken);
                if (validationResult == null)
                {
                    validationResult = WorkItemValidationResult.CreateFrom(validationArguments, isValid: true);

                    await stateStoreService.SaveWorkItemValidationResultAsync(validationResult, validationArguments, cancellationToken);
                }
                else
                {
                    logger.LogInformation("Validation result already exists for {BuildId}", validationArguments.BuildId);
                }

                await integrationService.PublishValidationCompletedEventAsync(validationResult, cancellationToken);
            }
            else
            {
                logger.LogWarning("Received empty or invalid validation request");
            }
            return new { Ok = true };
        }
    }
}
