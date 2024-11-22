

using System.Text.Json.Serialization;


namespace AzDO.PipelineChecks.Shared.ValidationDto
{
    public class ValidationCompletedEvent : ValidationResult
    {
        [JsonPropertyName("checkKind")]
        public CheckKind CheckKind { get; set; }
    }

    public enum CheckKind
    {
        WorkItem,
        Change
    }
}
