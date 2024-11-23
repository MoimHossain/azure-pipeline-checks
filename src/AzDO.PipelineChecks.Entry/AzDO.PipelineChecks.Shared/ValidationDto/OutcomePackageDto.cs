

using AzDO.PipelineChecks.Shared.Messaging;
using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared.ValidationDto
{
    public record OutcomePackageDto(
            [property: JsonPropertyName("httpHeaderCollection")] HttpHeaderCollection? HttpHeaderCollection,
            [property: JsonPropertyName("validationResult")] ValidationCompletedEvent? Event
        );
}
