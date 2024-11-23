

namespace AzDO.PipelineChecks.Shared
{
    public class ConfigReader
    {
        private const string APP_INSIGHT_CONN_ENV_KEY = "APP_INSIGHT_CONN";

        public string ApplicationInsightConnectionString { get; init; } = GetEnvVariable(APP_INSIGHT_CONN_ENV_KEY);

        // read env variable and if not exists return empty string
        private static string GetEnvVariable(string key)
        {
            try
            {
                return Environment.GetEnvironmentVariable(key) ?? string.Empty;
            }
            catch { }
            return string.Empty;
        }

        private ConfigReader() { }

        public static readonly ConfigReader Instance = new();
    }
}
