

using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared.ValidationDto
{
    public abstract class ValidationResult
    {
        [JsonPropertyName("checkComputation")]
        public CheckResultCompuationKind CheckComputation { get; set; }

        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; } = DateTime.Now;

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = [];

        [JsonPropertyName("projectId")]
        public Guid ProjectId { get; set; }

        [JsonPropertyName("buildId")]
        public int BuildId { get; set; }

        [JsonPropertyName("definitionId")]
        public int DefinitionId { get; set; }

        [JsonPropertyName("stageName")]
        public string? StageName { get; set; }

        [JsonPropertyName("jobId")]

        public Guid JobId { get; set; }

        [JsonPropertyName("stageId")]
        public Guid StageId { get; set; }

        [JsonPropertyName("planId")]
        public Guid PlanId { get; set; }

        [JsonPropertyName("checkStageId")]
        public Guid CheckStageId { get; set; }

        [JsonPropertyName("timelineId")]
        public Guid TimelineId { get; set; }
        [JsonPropertyName("taskInstanceId")]
        public Guid TaskInstanceId { get; set; }
        [JsonPropertyName("taskInstanceName")]
        public string? TaskInstanceName { get; set; }

    }

    public static class ValidationResultExtensions
    {
        public static void CopyFrom(this ValidationResult? validationResult, ValidationArguments validationArguments)
        {
            if (validationResult != null && validationArguments != null) 
            {
                validationResult.BuildId = validationArguments.BuildId;
                validationResult.JobId = validationArguments.JobId;
                validationResult.StageId = validationArguments.StageId;
                validationResult.PlanId = validationArguments.PlanId;
                validationResult.CheckStageId = validationArguments.CheckStageId;
                validationResult.ProjectId = validationArguments.ProjectId;
                validationResult.StageName = validationArguments.StageName;
                validationResult.DefinitionId = validationArguments.DefinitionId;
                validationResult.TimelineId = validationArguments.TimelineId;
                validationResult.TaskInstanceId = validationArguments.TaskInstanceId;
                validationResult.TaskInstanceName = validationArguments.TaskInstanceName;
            }
        }
    }
}
