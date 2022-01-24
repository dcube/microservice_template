using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.Configuration;
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
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IService1, Service1>();

            return services;
        }
    }
}
