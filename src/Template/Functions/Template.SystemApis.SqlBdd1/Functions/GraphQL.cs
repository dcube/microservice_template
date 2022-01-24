using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Template.GraphQL.Middleware.Abstraction;
using Template.Core.Token;
using System.Threading;
using Template.Domain;

namespace Template.SystemApis.SqlBdd1.Functions
{
    public class GraphQL
    {
        private readonly IGraphQLMiddlewareResolver _proxy;
        private readonly IAccessTokenProvider _accessTokenProvider;

        public GraphQL(IGraphQLMiddlewareResolver proxy, IAccessTokenProvider accessTokenProvider)
        {
            _proxy = proxy ?? throw new ApplicationException(nameof(proxy));
            _accessTokenProvider = accessTokenProvider ?? throw new ApplicationException(nameof(accessTokenProvider));
        }

        [FunctionName("GraphQL")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "graphQL")] HttpRequest request,
            CancellationToken cancellationToken,
            ILogger logger)
        {
            logger.LogInformation($"GraphQL Http Triggered");

            Result authorizationResult;
            if (!(authorizationResult = await _accessTokenProvider.ValidateTokenAsync(request)).IsSuccess)
            {
                return new ObjectResult(new { IsSuccess = false, Message = authorizationResult.Message }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            return await _proxy.ExecuteFunctionsQueryAsync(request.HttpContext, logger, cancellationToken);
        }
    }
}
