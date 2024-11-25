

using AzDO.PipelineChecks.Shared.ValidationDto;
using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared.Visualizations
{
    public class VisualizationDto
    {
        public static string GetRowKey(int buildId, Guid stageId, Guid jobId, CheckKind kind, CheckResultCompuationKind computeKind) 
        {
            var jobIdShort = jobId.ToString().Substring(0, 8);
            var stageIdShort = stageId.ToString().Substring(0, 8);
            return $"{buildId}-{stageIdShort}-{jobIdShort}-{kind.ToString()}-{computeKind.ToString()}";
        }

        [JsonPropertyName("buildId")]
        public int BuildId { get; set; }

        [JsonPropertyName("stageId")]
        public Guid StageID { get; set; }

        [JsonPropertyName("stageName")]
        public string? StageName { get; set; } 

        [JsonPropertyName("jobId")]
        public Guid JobId { get; set; }

        [JsonPropertyName("CheckKind")]
        public string? CheckKind { get; set; }

        [JsonPropertyName("CheckStatus")]
        public string? CompuationKind { get; set; }
    }
}
