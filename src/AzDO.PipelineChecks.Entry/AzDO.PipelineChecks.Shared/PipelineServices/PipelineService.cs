

using AzDO.PipelineChecks.Shared.Messaging;
using AzDO.PipelineChecks.Shared.Utils;
using AzDO.PipelineChecks.Shared.ValidationDto;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace AzDO.PipelineChecks.Shared.PipelineServices
{
    public class PipelineService(ILogger<PipelineService> logger)
    {
        public async Task ReportTaskBegingAsync(HttpHeaderCollection httpHeaderCollection, CancellationToken cancellationToken)
        {
            logger.LogInformation("Reporting Azure DevOps that task has been started..");

            await ExecuteInContextAsync(
                httpHeaderCollection,
                cancellationToken,
                async (taskLogger, taskClient, taskProperties) =>
            {
                await taskLogger.CreateTaskTimelineRecordIfRequired(taskClient, cancellationToken).ConfigureAwait(false);
                await taskLogger.LogImmediately("The validation workflow has started...please wait.");
                await taskClient.ReportTaskStarted(taskProperties.TaskInstanceId, cancellationToken).ConfigureAwait(false);
            });
        }

        public async Task ReportTaskProgressAsync(
            string message,
            HttpHeaderCollection httpHeaderCollection,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Reporting Azure DevOps that task is progressing..{message}", message);

            await ExecuteInContextAsync(
                httpHeaderCollection,
                cancellationToken,
                async (taskLogger, taskClient, taskProperties) =>
                {
                    await taskLogger.LogImmediately(message);
                    await taskClient.ReportTaskProgress(taskProperties.TaskInstanceId, cancellationToken).ConfigureAwait(false);
                });
        }

        public async Task ReportTaskCompletedAsync(
            string message,
            bool isSucceeded,
            HttpHeaderCollection httpHeaderCollection,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Reporting Azure DevOps that task is progressing..{message}", message);

            await ExecuteInContextAsync(
                httpHeaderCollection,
                cancellationToken,
                async (taskLogger, taskClient, taskProperties) =>
                {
                    var taskResult = isSucceeded ? TaskResult.Succeeded : TaskResult.Failed;

                    await taskLogger.LogImmediately(message);
                    await taskClient.ReportTaskCompleted(taskProperties.TaskInstanceId, taskResult, cancellationToken).ConfigureAwait(false);
                });
        }





        private async Task ExecuteInContextAsync(
            HttpHeaderCollection httpHeaderCollection,
            CancellationToken cancellationToken,
            Func<TaskLogService, TaskClient, ValidationArguments, Task> excuteCode)
        {
            logger.LogInformation("Reporting Azure DevOps that task has been started..");

            TaskLogService? taskLogger = null;
            try
            {
                var taskProperties = ValidationArguments.ReadFromRequestHeader(httpHeaderCollection);
                using var taskClient = new TaskClient(taskProperties);
                taskLogger = new TaskLogService(taskProperties, taskClient);

                await excuteCode(taskLogger, taskClient, taskProperties);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while Communicating to Azure DevOps");
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
