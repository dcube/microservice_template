using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using HotChocolate.Execution;
using HotChocolate.Language;
using HotChocolate.Server;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Template.GraphQL.Middleware.Abstraction;

namespace Template.GraphQL.Middleware
{
    public class GraphQLMiddlewareResolver : IGraphQLMiddlewareResolver
    {
        public IQueryExecutor Executor { get; }

        public IServiceProvider Services { get; }

        public IDocumentCache DocumentCache { get; }

        public IDocumentHashProvider DocumentHashProvider { get; }

        public IGraphQLMiddlewareOptions AzureFunctionsOptions { get; }

        private readonly RequestHelper requestHelper;
        private readonly JsonQueryResultSerializer jsonQueryResultSerializer;

        public GraphQLMiddlewareResolver(
            IQueryExecutor executor,
            IServiceProvider services,
            IDocumentCache documentCache,
            IDocumentHashProvider documentHashProvider,
            IGraphQLMiddlewareOptions azureFunctionsOptions)
        {
            Executor = executor;
            Services = services;
            DocumentCache = documentCache;
            DocumentHashProvider = documentHashProvider;
            AzureFunctionsOptions = azureFunctionsOptions;

            jsonQueryResultSerializer = new JsonQueryResultSerializer();

            requestHelper = new RequestHelper(DocumentCache, DocumentHashProvider, AzureFunctionsOptions.MaxRequestSize, AzureFunctionsOptions.ParserOptions);
        }

        public IActionResult DownloadSchema()
        {
            var schema = Executor.Schema.ToString();

            return new OkObjectResult(schema);
        }

        public async Task<IActionResult> ExecuteFunctionsQueryAsync(HttpContext context, ILogger logger, CancellationToken cancellationToken)
        {
            try
            {
                using var stream = context.Request.Body;

                var requestQuery = await requestHelper
                    .ReadJsonRequestAsync(stream, cancellationToken)
                    .ConfigureAwait(false);

                var builder = QueryRequestBuilder.New();

                if (requestQuery.Count > 0)
                {
                    var firstQuery = requestQuery[0];

                    builder
                        .SetQuery(firstQuery.Query)
                        .SetOperation(firstQuery.OperationName)
                        .SetQueryName(firstQuery.QueryName)
                        .SetServices(Services);

                    if (firstQuery.Variables?.Count > 0)
                    {
                        builder.SetVariableValues(firstQuery.Variables);
                    }
                }

                var result = await Executor.ExecuteAsync(builder.Create());

                if (result.Errors.Count > 0)
                {
                    throw new GraphQLMiddlewareException()
                    {
                        Errors = result.Errors
                    };
                }

                var resultString = jsonQueryResultSerializer.Serialize((IReadOnlyQueryResult)result);
                var resultObject = JsonConvert.DeserializeObject(resultString) as JObject;

                return new OkObjectResult(resultObject["data"]);
            }
            catch (GraphQLMiddlewareException ex)
            {
                logger.LogError(ex, $"Erreur lors de l'éxecution de la requête GraphQL | message d'erreur : {ex.Message}");

                return new BadRequestObjectResult(new
                {
                    Message = "Erreur lors de l'éxecution de la requête GraphQL",
                    Erreurs = ex.Errors
                });
            }
            catch (SyntaxException ex)
            {
                logger.LogError(ex, $"Erreur syntaxique lors de l'éxecution de la requête GraphQL | message d'erreur : {ex.Message}");

                return new BadRequestObjectResult(new
                {
                    Message = "Erreur syntaxique lors de l'éxecution de la requête GraphQL",
                    Erreur = new
                    {
                        ex.Line,
                        ex.Column,
                        ex.Position,
                        ex.Message
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erreur lors de l'éxection de la requête GraphQL | Message d'erreur : {ex.Message}");

                return new BadRequestObjectResult(new
                {
                    Message = ex.Message
                });
            }
        }
    }
}
