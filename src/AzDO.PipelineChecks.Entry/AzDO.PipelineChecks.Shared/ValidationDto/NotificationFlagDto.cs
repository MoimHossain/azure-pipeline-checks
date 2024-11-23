

using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared.ValidationDto
{
    public class NotificationFlagDto
    {
        [JsonPropertyName("notified")]
        public bool Notified { get; set; } = true;
    }
}
