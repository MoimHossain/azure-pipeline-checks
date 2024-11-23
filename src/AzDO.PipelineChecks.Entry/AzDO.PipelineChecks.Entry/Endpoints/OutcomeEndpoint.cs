
using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.PipelineServices;
using AzDO.PipelineChecks.Shared.StateManagement;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Microsoft.AspNetCore.Mvc;

namespace AzDO.PipelineChecks.Entry.Endpoints
{
    public class OutcomeEndpoint
    {
        public static async Task<object> Handler(
            [FromBody] Envelope<OutcomePackageDto> envelope,
            [FromServices] StateStoreService stateStoreService,
            [FromServices] PipelineService pipelineService,
            ILogger<OutcomeEndpoint> logger,
            CancellationToken cancellationToken)
        {
            OutcomeDto? outcomeDto;

            if (envelope != null && envelope.Data != null && envelope.Data.HttpHeaderCollection != null && envelope.Data.Event != null)
            {
                var httpHeaders = envelope.Data.HttpHeaderCollection;
                var outcomeEvent = envelope.Data.Event;

                var validationResultInString = outcomeEvent.IsValid ? "PASSED" : "FAILED";
                logger.LogInformation("Received outcome response: {changeKind} {Result}", outcomeEvent.CheckKind.ToString(), validationResultInString);

                outcomeDto = await stateStoreService.GetOutcomeAsync(outcomeEvent.DefinitionId, outcomeEvent.BuildId, cancellationToken);

                if (outcomeDto == null) 
                {
                    outcomeDto = new OutcomeDto
                    {
                        ProjectId = outcomeEvent.ProjectId,
                        BuildId = outcomeEvent.BuildId,
                        DefinitionId = outcomeEvent.DefinitionId
                    };
                }

                // delete any old check outcomes
                outcomeDto.CheckOutcomes.RemoveAll(x => x.CheckKind == outcomeEvent.CheckKind);
                outcomeDto.CheckOutcomes.Add(new CheckOutcomeDto
                {
                    CheckKind = outcomeEvent.CheckKind,
                    IsValid = outcomeEvent.IsValid,
                    Errors = outcomeEvent.Errors
                });
                await stateStoreService.SaveOutcomeAsync(outcomeDto, cancellationToken);

                List<CheckKind> mandatoryChecks = [CheckKind.WorkItem, CheckKind.Change];
                
                var allMandatoryChecksPerformed = (mandatoryChecks.All(x => outcomeDto.CheckOutcomes.Any(y => y.CheckKind == x)));
                var allChecksValid = outcomeDto.CheckOutcomes.All(x => x.IsValid);

                if(allMandatoryChecksPerformed)
                {
                    var message = allChecksValid ? "All checks completed successfully" : "Some (or all) checks failed";
                    await pipelineService.ReportTaskCompletedAsync(message, allChecksValid, httpHeaders, cancellationToken);
                }
            }
            else
            {
                logger.LogWarning("Received empty or invalid validation request");
            }
            return new { Ok = true };
        }
    }
}
