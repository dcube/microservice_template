using HotChocolate;

namespace Template.GraphQL.Middleware
{
    public class GraphQLMiddlewareException : Exception
    {
        public IReadOnlyCollection<IError> Errors { get; set; }

        public GraphQLMiddlewareException()
        {
        }

        public GraphQLMiddlewareException(string message) : base(message)
        {
        }

        public GraphQLMiddlewareException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
