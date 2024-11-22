
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AzDO.PipelineChecks.Shared.Utils
{
    public class HttpHeaderTraceClient(ILogger<HttpHeaderTraceClient> logger)
    {
        public void TraceHeaders(HttpContext context)
        {
            var logmessage = new StringBuilder();
            foreach (var header in context.Request.Headers)
            {                
                var value = header.Value.ToString();
                if (value.Length > 60)
                {
                    value = value.Substring(0, 60) + "...";
                }
                logmessage.AppendLine($"{header.Key}: {value}");
            }
            logger.LogInformation("Received request with headers: {headers}", logmessage);
        }
    }
}
