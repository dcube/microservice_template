using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.Configuration;
using Template.GraphQL.Extensions;
using Template.SystemApis.SqlBdd1.GraphQL;
using Template.SystemApis.SqlBdd1.Repositories.Context;

[assembly: FunctionsStartup(typeof(Template.SystemApis.SqlBdd1.Startup))]
namespace Template.SystemApis.SqlBdd1
{
    
    public class Startup : AzureStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            base.Configure(builder);

            ConfigureGraphQL(builder.Services);
            ConfigureServices(builder.Services);
            ConfigureContext(builder.Services, configuration);
        }

        private void ConfigureGraphQL(IServiceCollection services)
        {
            services.AddAzureFunctionsGraphQL<Query>();
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {

            return services;
        }

        public IServiceCollection ConfigureContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SqlBdd1Context>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"]);
            }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

            return services;
        }
    }
}
