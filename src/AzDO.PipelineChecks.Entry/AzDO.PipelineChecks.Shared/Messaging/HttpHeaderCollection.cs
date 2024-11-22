

using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared.Messaging
{
    public class HttpHeaderCollection
    {
        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; } = [];

    }

    public static class HttpHeaderCollectionExtensions
    {
        public static HttpHeaderCollection From(this IHeaderDictionary headers)
        {
            var collection = new HttpHeaderCollection();
            foreach (var header in headers)
            {
                collection.Headers.Add(header.Key, header.Value.ToString());
            }
            return collection;
        }
    }
}
