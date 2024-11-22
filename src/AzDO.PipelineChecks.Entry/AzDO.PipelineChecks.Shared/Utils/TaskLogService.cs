

using AzDO.PipelineChecks.Shared.ValidationDto;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Text;

namespace AzDO.PipelineChecks.Shared.Utils
{
    public class TaskLogService
    {
        private readonly ValidationArguments payload;

        private int byteCount;
        
        private readonly string pageId;
        private MemoryStream? pageData;
        private StreamWriter? pageWriter;
        private readonly TaskClient taskClient;

        // 8 MB
        private const int PageSize = 8 * 1024 * 1024;

        public TaskLogService(ValidationArguments payload, TaskClient taskClient)
        {
            this.payload = payload;
            this.taskClient = taskClient;
            pageId = Guid.NewGuid().ToString();
        }

        public async Task Log(string message)
        {
            if (!string.IsNullOrEmpty(message) && message.Length > 1024)
            {
                Console.WriteLine("Web console line is more than 1024 chars, truncate to first 1024 chars");
                message = $"{message.Substring(0, 1024)}...";
            }

            var line = $"{DateTime.UtcNow:O} {message}";
            await LogPage(line).ConfigureAwait(false);
        }

        public async Task LogImmediately(string message)
        {
            await Log(message);
            await End();
        }

        public async Task End()
        {
            await EndPage().ConfigureAwait(false);
        }

        private async Task LogPage(string message)
        {
            // lazy creation on write
            if (pageWriter == null)
            {
                await NewPage().ConfigureAwait(false);
            }

            if (pageWriter != null)
            {
                pageWriter.WriteLine(message);
                byteCount += Encoding.UTF8.GetByteCount(message);
                if (byteCount >= PageSize)
                {
                    await NewPage().ConfigureAwait(false);
                }
            }
        }

        private async Task NewPage()
        {
            await EndPage().ConfigureAwait(false);
            byteCount = 0;
            pageData = new MemoryStream();
            pageWriter = new StreamWriter(pageData, Encoding.UTF8);
        }

        private async Task EndPage()
        {
            if (pageWriter != null && pageData != null)
            {
                pageWriter.Flush();
                pageData.Seek(0, SeekOrigin.Begin);
                var log = new TaskLog(string.Format(@"logs\{0:D}", payload.TaskInstanceId));
                var taskLog = await taskClient.CreateLogAsync(log).ConfigureAwait(false);

                // Upload the contents
                await taskClient.AppendLogContentAsync(taskLog.Id, pageData).ConfigureAwait(false);

                // Create a new record and only set the Log field
                var attachmentUpdataRecord = new TimelineRecord { Id = payload.TaskInstanceId, Log = taskLog };
                await taskClient.UpdateTimelineRecordsAsync(attachmentUpdataRecord, default).ConfigureAwait(false);

                pageWriter.Dispose();
                pageWriter = null;
                pageData = null;
            }
        }

        public async Task CreateTaskTimelineRecordIfRequired(TaskClient taskClient, CancellationToken cancellationToken)
        {
            if (payload.TaskInstanceId.Equals(Guid.Empty))
            {
                payload.TaskInstanceId = Guid.NewGuid();
            }

            var timelineRecord = new TimelineRecord
            {
                Id = payload.TaskInstanceId,
                RecordType = "task",
                StartTime = DateTime.UtcNow,
                ParentId = payload.JobId,
            };

            if (!string.IsNullOrWhiteSpace(payload.TaskInstanceName))
            {
                timelineRecord.Name = payload.TaskInstanceName;
            }

            // this is an upsert call
            await taskClient.UpdateTimelineRecordsAsync(timelineRecord, cancellationToken).ConfigureAwait(false);
        }
    }
}
