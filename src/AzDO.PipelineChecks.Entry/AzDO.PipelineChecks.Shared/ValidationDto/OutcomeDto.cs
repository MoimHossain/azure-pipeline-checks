
using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared.ValidationDto
{
    public class OutcomeDto
    {
        [JsonPropertyName("projectId")]
        public Guid ProjectId { get; set; }

        [JsonPropertyName("buildId")]
        public int BuildId { get; set; }

        [JsonPropertyName("definitionId")]
        public int DefinitionId { get; set; }

        [JsonPropertyName("checkOutcomes")]
        public List<CheckOutcomeDto> CheckOutcomes { get; set; } = [];
    }

    public class CheckOutcomeDto
    {
        [JsonPropertyName("checkKind")]
        public CheckKind CheckKind { get; set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = [];
    }
}
