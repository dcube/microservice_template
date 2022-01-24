using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.Token;

namespace Template.Core.Configuration
{
    public class AzureStartup : FunctionsStartup
    {
        private const string appConfigurationEndpoint = "AppConfigurationEndpoint";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            ConfigureAuthorization(services);
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            string? endpoint = Environment.GetEnvironmentVariable(appConfigurationEndpoint);
            if(string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException(nameof(endpoint));
            }

            builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
            {
                options
                .Connect(new Uri(endpoint), new DefaultAzureCredential())
                .UseFeatureFlags(featureFlagOptions =>
                {
                    featureFlagOptions.CacheExpirationInterval = TimeSpan.FromMinutes(10);
                });
            });
#if DEBUG
            string directory = builder.GetContext().ApplicationRootPath;
            string settingsPath = Path.Combine(directory, "local.settings.json");

            builder.ConfigurationBuilder
                .AddJsonFile(settingsPath)
                .Build();
#endif
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
        }
    }
}
