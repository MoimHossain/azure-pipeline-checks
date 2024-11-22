

using AzDO.PipelineChecks.Shared;
using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.Payloads;
using AzDO.PipelineChecks.Shared.Utils;

namespace AzDO.PipelineChecks.Entry.Endpoints
{
    public class CheckEntryEndpoint
    {
        public static async Task<IResult> Handler(
            HttpContext context,
            IntegrationService integrationService,
            HttpHeaderTraceClient httpHeaderTraceClient,
            ILogger<CheckEntryEndpoint> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                httpHeaderTraceClient.TraceHeaders(context);

                var payload = RequestPayload.ReadFromRequestHeader(context.Request.Headers);

                var envelope = new Envelope<RequestPayload>(payload);
                await integrationService.InvokeAsync(envelope, cancellationToken);
                return Results.Accepted<string>(value: "hi");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error processing request");
                return Results.Problem(e.Message); // fix this later
            }
        }
    }
}
















//public class TaskExecution
//{
//    public async Task ExecuteAsync(RequestPayload taskProperties, CancellationToken cancellationToken)
//    {
//        TaskLogService? taskLogger = null;
//        using var taskClient = new TaskClient(taskProperties);
//        var taskResult = TaskResult.Succeeded;
//        try
//        {
//            // create timeline record if not provided
//            taskLogger = new TaskLogService(taskProperties, taskClient);
//            await taskLogger.CreateTaskTimelineRecordIfRequired(taskClient, cancellationToken).ConfigureAwait(false);

//            // report task started
//            await taskLogger.LogImmediately("Task started");
//            await taskClient.ReportTaskStarted(taskProperties.TaskInstanceId, cancellationToken).ConfigureAwait(false);
//            await Task.Delay(1000);
//            await taskClient.ReportTaskProgress(taskProperties.TaskInstanceId, cancellationToken).ConfigureAwait(false);

//            await Task.Delay(1000);
//            // report task completed with status
//            await taskLogger.LogImmediately("Task completed");
//            await taskClient.ReportTaskCompleted(taskProperties.TaskInstanceId, taskResult, cancellationToken).ConfigureAwait(false);
//        }
//        catch (Exception e)
//        {
//            if (taskLogger != null)
//            {
//                await taskLogger.Log(e.ToString()).ConfigureAwait(false);
//            }

//            await taskClient.ReportTaskCompleted(taskProperties.TaskInstanceId, taskResult, cancellationToken).ConfigureAwait(false);
//            throw;
//        }
//        finally
//        {
//            if (taskLogger != null)
//            {
//                await taskLogger.End().ConfigureAwait(false);
//            }
//        }
//    }
//}