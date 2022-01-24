using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Template.GraphQL.Middleware.Abstraction;

namespace Template.SystemApis.Functions
{
    public class GraphQLSchema
    {
        private readonly IGraphQLMiddlewareResolver proxy;

        public GraphQLSchema(IGraphQLMiddlewareResolver proxy)
        {
            this.proxy = proxy;
        }

        [FunctionName(nameof(GraphQLSchema))]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "graphQL/schema")] HttpRequest request, 
            ILogger logger)
        {
            return proxy.DownloadSchema();
        }
    }
}
