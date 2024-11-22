

using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Google.Api;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AzDO.Pipelines.WorkItemValidation.Endpoints
{
    public class ValidationEndpoint
    {
        public static async Task<object> Handler(            
            [FromBody]Envelope<HttpHeaderCollection> envelope,
            ILogger<ValidationEndpoint> logger,
            CancellationToken cancellationToken)
        {
            if (envelope != null && envelope.Data != null) 
            {
                var validationArguments = ValidationArguments.ReadFromRequestHeader(envelope.Data);
                await Task.CompletedTask;
                logger.LogInformation("Validation arguments: {arguments}", JsonSerializer.Serialize(validationArguments));
            }
            return new { Ok = true };
        }
    }
}
