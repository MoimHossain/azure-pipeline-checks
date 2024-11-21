

using AzDO.PipelineChecks.Shared.Payloads;
using AzDO.PipelineChecks.Shared.Utils;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Text;

namespace AzDO.PipelineChecks.Entry.Endpoints
{
    public class CheckEntryEndpoint
    {
        public static async Task<IResult> Handler(
            HttpContext context,
            ILogger<CheckEntryEndpoint> logger,
            CancellationToken cancellationToken)
        {
            var payload = RequestPayload.ReadFromRequestHeader(context.Request.Headers);

            var logmessage = new StringBuilder();
            foreach (var header in context.Request.Headers)
            {
                // trucate the value if it more than 200 chars
                var value = header.Value.ToString();
                if (value.Length > 60)
                {
                    value = value.Substring(0, 60) + "...";
                }

                logmessage.AppendLine($"{header.Key}: {value}");
            }

            logger.LogInformation("Received request with headers: {headers}", logmessage);

            var executionEngine = new TaskExecution();
            _ = Task.Run(() => executionEngine.ExecuteAsync(payload, cancellationToken));

            await Task.CompletedTask;

            return Results.Accepted<string>(value: "hi");
        }
    }

    public class TaskExecution
    {
        public async Task ExecuteAsync(RequestPayload taskProperties, CancellationToken cancellationToken)
        {
            TaskLogService? taskLogger = null;
            using var taskClient = new TaskClient(taskProperties);
            var taskResult = TaskResult.Succeeded;
            try
            {
                // create timeline record if not provided
                taskLogger = new TaskLogService(taskProperties, taskClient);
                await taskLogger.CreateTaskTimelineRecordIfRequired(taskClient, cancellationToken).ConfigureAwait(false);

                // report task started
                await taskLogger.LogImmediately("Task started");
                await taskClient.ReportTaskStarted(taskProperties.TaskInstanceId, cancellationToken).ConfigureAwait(false);
                await Task.Delay(1000);
                await taskClient.ReportTaskProgress(taskProperties.TaskInstanceId, cancellationToken).ConfigureAwait(false);

                await Task.Delay(1000);
                // report task completed with status
                await taskLogger.LogImmediately("Task completed");
                await taskClient.ReportTaskCompleted(taskProperties.TaskInstanceId, taskResult, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (taskLogger != null)
                {
                    await taskLogger.Log(e.ToString()).ConfigureAwait(false);
                }

                await taskClient.ReportTaskCompleted(taskProperties.TaskInstanceId, taskResult, cancellationToken).ConfigureAwait(false);
                throw;
            }
            finally
            {
                if (taskLogger != null)
                {
                    await taskLogger.End().ConfigureAwait(false);
                }
            }
        }
    }
}