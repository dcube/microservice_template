using HotChocolate;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.GraphQL.Middleware;
using Template.GraphQL.Middleware.Abstraction;

namespace Template.GraphQL.Extensions
{
    public static class GraphQlMiddlewareExtension
    {
        public static IServiceCollection AddAzureFunctionsGraphQL<TQuery>(this IServiceCollection services, IGraphQLMiddlewareOptions options)
            where TQuery : class, IGraphQLMiddlewareQuery
        {
            services.AddGraphQL(sp => SchemaBuilder.New()
                .AddQueryType<TQuery>()
                .Create(), new QueryExecutionOptions() { ForceSerialExecution = true, TracingPreference = TracingPreference.Never });

            services.AddSingleton(options);

            services.AddSingleton<IGraphQLMiddlewareResolver, GraphQLMiddlewareResolver>();

            return services;
        }

        public static IServiceCollection AddAzureFunctionsGraphQL<TQuery, TMutation>(this IServiceCollection services, IGraphQLMiddlewareOptions options)
            where TQuery : class, IGraphQLMiddlewareQuery
            where TMutation : class, IGraphQLMiddlewareMutation
        {
            services.AddGraphQL(sp => SchemaBuilder.New()
                .AddQueryType<TQuery>()
                .AddMutationType<TMutation>()
                .Create(), new QueryExecutionOptions() { ForceSerialExecution = true, TracingPreference = TracingPreference.Never });

            services.AddSingleton(options);

            services.AddSingleton<IGraphQLMiddlewareResolver, GraphQLMiddlewareResolver>();

            return services;
        }

        public static IServiceCollection AddAzureFunctionsGraphQL<TQuery>(this IServiceCollection services)
            where TQuery : class, IGraphQLMiddlewareQuery
        {
            services.AddAzureFunctionsGraphQL<TQuery>(new GraphQlMiddlewareOptions());

            return services;
        }

        public static IServiceCollection AddAzureFunctionsGraphQL<TQuery, TMutation>(this IServiceCollection services)
            where TQuery : class, IGraphQLMiddlewareQuery
            where TMutation : class, IGraphQLMiddlewareMutation
        {
            services.AddAzureFunctionsGraphQL<TQuery, TMutation>(new GraphQlMiddlewareOptions());

            return services;
        }
    }
}
