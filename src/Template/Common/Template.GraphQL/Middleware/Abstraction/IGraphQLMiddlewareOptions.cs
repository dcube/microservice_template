using HotChocolate.AspNetCore;

namespace Template.GraphQL.Middleware.Abstraction
{
    public interface IGraphQLMiddlewareOptions : IParserOptionsAccessor
    {
        int MaxRequestSize { get; }
    }
}
