

using AzDO.PipelineChecks.Shared.Payloads;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzDO.PipelineChecks.Shared.Utils
{
    public class TaskClient : IDisposable
    {
        private readonly RequestPayload taskProperties;
        private TaskHttpClient taskClient;
        private VssConnection vssConnection;
        private int? PlanVersion;

        public RequestPayload Payload => taskProperties;

        public TaskClient(RequestPayload payload)
        {
            this.taskProperties = payload;
            var vssBasicCredential = new VssBasicCredential(string.Empty, payload.AuthToken);
            vssConnection = new VssConnection(payload.PlanUri, vssBasicCredential);
            taskClient = vssConnection.GetClient<TaskHttpClient>();
        }

        public async Task ReportTaskAssigned(Guid taskId, CancellationToken cancellationToken)
        {
            var jobId = await GetJobId(this.Payload.HubName, this.Payload.JobId, taskId);
            taskId = await GetTaskId(this.Payload.HubName, taskId);

            var startedEvent = new TaskAssignedEvent(jobId, taskId);
            await taskClient.RaisePlanEventAsync(Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, startedEvent, cancellationToken).ConfigureAwait(false);
        }

        public async Task ReportTaskStarted(Guid taskId, CancellationToken cancellationToken)
        {
            var jobId = await GetJobId(this.Payload.HubName, this.Payload.JobId, taskId);
            taskId = await GetTaskId(this.Payload.HubName, taskId);

            var startedEvent = new TaskStartedEvent(jobId, taskId);
            await taskClient.RaisePlanEventAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, startedEvent, cancellationToken).ConfigureAwait(false);
        }

        public async Task ReportTaskProgress(Guid taskId, CancellationToken cancellationToken)
        {
            var records = await taskClient.GetRecordsAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, this.Payload.TimelineId, userState: null, cancellationToken: cancellationToken).ConfigureAwait(false);
            var taskRecord = records.FirstOrDefault(r => r.Id == taskId);
            if(taskRecord != null)
            {
                taskRecord.State = TimelineRecordState.InProgress;

                await taskClient.UpdateTimelineRecordsAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, this.Payload.TimelineId, new List<TimelineRecord> { taskRecord }, cancellationToken).ConfigureAwait(false);
            }            
        }

        public async Task ReportTaskCompleted(Guid taskId, TaskResult result, CancellationToken cancellationToken)
        {
            var jobId = await GetJobId(this.Payload.HubName, this.Payload.JobId, taskId);
            taskId = await GetTaskId(this.Payload.HubName, taskId);

            var completedEvent = new TaskCompletedEvent(jobId, taskId, result);
            await taskClient.RaisePlanEventAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, completedEvent, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateTimelineRecordsAsync(TimelineRecord timelineRecord, CancellationToken cancellationToken)
        {
            await taskClient.UpdateTimelineRecordsAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, this.Payload.TimelineId, new List<TimelineRecord> { timelineRecord }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TaskLog> CreateLogAsync(TaskLog log)
        {
            return await taskClient.CreateLogAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, log).ConfigureAwait(false);
        }

        public async Task<TaskLog> AppendLogContentAsync(int logId, Stream uploadStream)
        {
            return await taskClient.AppendLogContentAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, logId, uploadStream).ConfigureAwait(false);
        }

        public async Task SetTaskVariable(Guid taskId, string name, string value, bool isSecret, CancellationToken cancellationToken)
        {
            var records = await taskClient.GetRecordsAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, this.Payload.TimelineId, userState: null, cancellationToken: cancellationToken).ConfigureAwait(false);
            var taskRecord = records.FirstOrDefault(r => r.Id == taskId);
            if(taskRecord != null)
            {
                taskRecord.Variables[name] = new VariableValue { Value = value, IsSecret = isSecret };

                await taskClient.UpdateTimelineRecordsAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, this.Payload.TimelineId, new List<TimelineRecord> { taskRecord }, cancellationToken).ConfigureAwait(false);
            }
        }

        public string? GetTaskVariable(Guid taskId, string name, CancellationToken cancellationToken)
        {
            var records = taskClient.GetRecordsAsync(this.Payload.ProjectId, this.Payload.HubName, this.Payload.PlanId, this.Payload.TimelineId, userState: null, cancellationToken: cancellationToken).Result;
            var taskRecord = records.FirstOrDefault(r => r.Id == taskId);
            if(taskRecord != null && taskRecord.Variables != null)
            {
                foreach (var variable in taskRecord.Variables)
                {
                    if (string.Equals(variable.Key, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return variable.Value.Value;
                    }
                }
            }
            return null;
        }

        public void Dispose()
        {
            vssConnection.Dispose();
            taskClient.Dispose();
        }

        private async Task<Guid> GetJobId(string? hubName, Guid jobId, Guid taskId)
        {
            if (!string.IsNullOrWhiteSpace(hubName) 
                && hubName.Equals("Gates", StringComparison.OrdinalIgnoreCase))
            {
                var planVersion = await GetPlanVersion();
                if (planVersion <= 12)
                {
                    return taskId;
                }
            }

            return jobId;
        }

        private async Task<Guid> GetTaskId(string? hubName, Guid taskId)
        {
            if (!string.IsNullOrWhiteSpace(hubName)
                && hubName.Equals("Gates", StringComparison.OrdinalIgnoreCase))
            {
                var planVersion = await GetPlanVersion();
                if (planVersion <= 12)
                {
                    return Guid.Empty;
                }
            }

            return taskId;
        }

        private async Task<int> GetPlanVersion()
        {
            if (this.PlanVersion == null)
            {
                var plan = await taskClient.GetPlanAsync(Payload.ProjectId, Payload.HubName, Payload.PlanId);
                PlanVersion = plan.Version;
            }

            return PlanVersion.Value;
        }
    }
}
