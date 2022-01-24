
using HotChocolate.Execution;
using HotChocolate.Language;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Template.GraphQL.Middleware.Abstraction
{
    public interface IGraphQLMiddlewareResolver
    {
        IQueryExecutor Executor { get; }

        IDocumentCache DocumentCache { get; }

        IDocumentHashProvider DocumentHashProvider { get; }

        IGraphQLMiddlewareOptions AzureFunctionsOptions { get; }

        IActionResult DownloadSchema();

        Task<IActionResult> ExecuteFunctionsQueryAsync(HttpContext context, ILogger logger, CancellationToken cancellationToken);
    }
}
