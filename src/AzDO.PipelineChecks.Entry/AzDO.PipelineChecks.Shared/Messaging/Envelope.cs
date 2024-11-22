

using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared.Messaging
{
    public class Envelope<TPayloadType>
    {
        public Envelope()
        {

        }

        public Envelope(TPayloadType payload)
        {
            this.Data = payload;
        }

        [JsonPropertyName("data")]
        public TPayloadType? Data { get; set; }

        [JsonPropertyName("datacontenttype")]
        public string? Datacontenttype { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("pubsubname")]
        public string? Pubsubname { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("specversion")]
        public string? Specversion { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("topic")]
        public string? Topic { get; set; }

        [JsonPropertyName("traceid")]
        public string? Traceid { get; set; }

        [JsonPropertyName("traceparent")]
        public string? Traceparent { get; set; }

        [JsonPropertyName("tracestate")]
        public string? Tracestate { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
