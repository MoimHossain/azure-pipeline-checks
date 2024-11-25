

using AzDO.PipelineChecks.Shared.PipelineServices;
using AzDO.PipelineChecks.Shared.StateManagement;
using AzDO.PipelineChecks.Shared.Utils;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Dapr.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzDO.PipelineChecks.Shared
{
    public static class SharedExtensions
    {
        public static async Task<WebApplication> BuidAndConfigureAppAsync(this WebApplicationBuilder builder, string serviceName)
        {
            await builder.RegisterSharedServicesAsync(serviceName);

            var app = builder.Build();

            app.UseSharedMiddlewares();

            return app;
        }

        public static WebApplication UseSharedMiddlewares(this WebApplication webApp)
        {
            webApp.UseSwagger();
            webApp.UseSwaggerUI();
            webApp.UseCors();
            webApp.UseRouting();
            //webApp.UseHttpsRedirection();
            return webApp;
        }

        public static async Task RegisterSharedServicesAsync(
            this WebApplicationBuilder builder,
            string serviceName)
        {
            var services = builder.Services;
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

            services.AddApplicationInsightsTelemetry(aiOption =>
            {
                aiOption.ConnectionString = ConfigReader.Instance.ApplicationInsightConnectionString;
            });

            services.AddLogging(logging =>
            {
                logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddApplicationInsights();
                logging.AddDebug();
            });

            services.AddOpenTelemetry()
                .ConfigureResource(resBuilder => resBuilder
                    .AddService(serviceName))
                .WithLogging(logging => logging
                    .AddConsoleExporter())
                .WithTracing(tracing => tracing
                    .AddSource(serviceName)
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter())
                .WithMetrics(metrics => metrics
                    .AddMeter(serviceName)
                    .AddConsoleExporter())
                .UseAzureMonitor(azMonitorOption => azMonitorOption.ConnectionString = ConfigReader.Instance.ApplicationInsightConnectionString);

            builder.Logging.AddOpenTelemetry(options => options
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: serviceName))
                .AddConsoleExporter());

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
            services.AddSingleton<ConcurrentLeaseStore>();
            services.AddSingleton<VisualizationStore>();
            services.AddSingleton<PipelineService>();
            await Task.CompletedTask;
        }
    }
}
