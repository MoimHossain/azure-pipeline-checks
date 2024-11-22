

using AzDO.PipelineChecks.Shared.StateManagement;
using AzDO.PipelineChecks.Shared.Utils;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared
{
    public static class SharedExtensions
    {
        public static async Task RegisterSharedServicesAsync(
            this IServiceCollection services, IConfigurationManager configurationManager)
        {
            if(services != null)
            {
                services.AddEndpointsApiExplorer();
                services.AddHttpClient();
                services.AddSwaggerGen();



                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });
                
                services.AddLogging(logging =>
                {
                    logging.AddConfiguration(configurationManager.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                });
                services.AddSingleton(services =>
                {
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true,
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true
                    };
                    jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    return jsonSerializerOptions;
                });

                services.AddSingleton(new DaprClientBuilder().Build());
                services.AddSingleton<HttpHeaderTraceClient>();
                services.AddSingleton<IntegrationService>();
                services.AddSingleton<StateStoreService>();
            }
            await Task.CompletedTask;
        }
    }
}
