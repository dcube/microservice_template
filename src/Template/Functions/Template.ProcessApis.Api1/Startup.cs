using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Refit;
using Template.Core.Configuration;
using Template.Core.Extensions;
using Template.Core.Resolvers;
using Template.Domain.Constants;
using Template.ProcessApis.Api1.Services;

[assembly: FunctionsStartup(typeof(Template.ProcessApis.Api1.Startup))]
namespace Template.ProcessApis.Api1
{
    
    public class Startup : AzureStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            base.Configure(builder);

            ConfigureServices(builder.Services);
            builder.Services.ConfigureFunctionClient<IOrderSystemApiFunction>(configuration[GlobalConfigurationKeys.OrderSystemApiUrl], configuration[GlobalConfigurationKeys.OrderSystemApiKey]);
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IService1, Service1>();

            return services;
        }
    }
}
