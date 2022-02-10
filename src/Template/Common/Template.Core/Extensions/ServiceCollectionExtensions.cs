using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Refit;
using Template.Core.Formatters;
using Template.Core.Handlers;
using Template.Core.Resolvers;

namespace Template.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static RefitSettings GlobalSettings = new RefitSettings()
        {
            CollectionFormat = CollectionFormat.Multi,
            UrlParameterFormatter = new UrlFormatter(),
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings()
            {
                ContractResolver = new NonPublicPropertiesResolver()
            })
        };

        public static IServiceCollection ConfigureFunctionClient<T>(this IServiceCollection services, string functionUrl, string functionKey) where T : class
        {
            services.AddRefitClient<T>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(functionUrl))
                .AddHttpMessageHandler(_ => new AuthorizationHandler(functionKey));

            return services;
        }
    }
}
