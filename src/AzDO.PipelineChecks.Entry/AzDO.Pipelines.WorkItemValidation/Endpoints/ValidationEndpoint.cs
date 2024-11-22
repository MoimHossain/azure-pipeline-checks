

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
            HttpContext context,
            ILogger<ValidationEndpoint> logger,
            CancellationToken cancellationToken)
        {
            // read the body
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            logger.LogInformation("Received request with body: {body}", body);

            //if (envelope != null) 
            {
                

                //var validationArguments = ValidationArguments.ReadFromRequestHeader(envelope.Data);
                await Task.CompletedTask;
                //logger.LogInformation("Validation arguments: {arguments}", JsonSerializer.Serialize(validationArguments));
            } 
            

            return new { Ok = true };
        }
    }
}
