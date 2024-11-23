

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

                var validationResult = await stateStoreService.GetWorkItemValidationResultAsync(validationArguments, cancellationToken);
                if (validationResult == null)
                {
                    // TODO - Implement validation logic here                    
                    // Delay randomly between 1 and 5 seconds
                    var delay = new Random().Next(1, 5);
                    await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

                    validationResult = WorkItemValidationResult.CreateFrom(validationArguments, isValid: true);

                    await stateStoreService.SaveWorkItemValidationResultAsync(validationResult, validationArguments, cancellationToken);
                    await pipelineService.ReportTaskProgressAsync("Work item validation completed", envelope.Data, cancellationToken);
                }
                else
                {
                    logger.LogInformation("Validation result already exists for {BuildId}", validationArguments.BuildId);
                    await pipelineService.ReportTaskProgressAsync("Work item validation (skipping)", envelope.Data, cancellationToken);
                }

                await integrationService.PublishValidationCompletedEventAsync(CheckKind.WorkItem, validationResult, envelope.Data, cancellationToken);
            }
            else
            {
                logger.LogWarning("Received empty or invalid validation request");
            }
            return new { Ok = true };
        }
    }
}
