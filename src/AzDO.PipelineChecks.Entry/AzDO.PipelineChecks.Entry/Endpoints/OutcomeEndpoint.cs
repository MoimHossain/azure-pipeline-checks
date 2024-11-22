
using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.StateManagement;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Microsoft.AspNetCore.Mvc;

namespace AzDO.PipelineChecks.Entry.Endpoints
{
    public class OutcomeEndpoint
    {
        public static async Task<object> Handler(
            [FromBody] Envelope<ValidationCompletedEvent> envelope,
            [FromServices] StateStoreService stateStoreService,
            ILogger<OutcomeEndpoint> logger,
            CancellationToken cancellationToken)
        {
            OutcomeDto? outcomeDto;
            if (envelope != null && envelope.Data != null)
            {
                logger.LogInformation("Received outcome response: {changeKind}", envelope.Data.CheckKind.ToString());

                outcomeDto = await stateStoreService.GetOutcomeAsync(envelope.Data.DefinitionId, envelope.Data.BuildId, cancellationToken);

                if (outcomeDto == null) 
                {
                    outcomeDto = new OutcomeDto
                    {
                        ProjectId = envelope.Data.ProjectId,
                        BuildId = envelope.Data.BuildId,
                        DefinitionId = envelope.Data.DefinitionId
                    };
                }

                // delete any old check outcomes
                outcomeDto.CheckOutcomes.RemoveAll(x => x.CheckKind == envelope.Data.CheckKind);
                outcomeDto.CheckOutcomes.Add(new CheckOutcomeDto
                {
                    CheckKind = envelope.Data.CheckKind,
                    IsValid = envelope.Data.IsValid,
                    Errors = envelope.Data.Errors
                });

                await stateStoreService.SaveOutcomeAsync(outcomeDto, cancellationToken);

                await Task.CompletedTask;
            }
            else
            {
                logger.LogWarning("Received empty or invalid validation request");
            }
            return new { Ok = true };
        }
    }
}
