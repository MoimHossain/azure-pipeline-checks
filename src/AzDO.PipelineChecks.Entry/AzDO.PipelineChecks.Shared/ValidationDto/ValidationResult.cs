

using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared.ValidationDto
{
    public abstract class ValidationResult
    {
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
    }
}
