using HotChocolate.Language;

using Template.GraphQL.Middleware.Abstraction;

namespace Template.GraphQL.Middleware
{
    public class GraphQlMiddlewareOptions : IGraphQLMiddlewareOptions
    {
        private const int minMaxRequestSize = 1024;

        private int maxRequestSize = 20 * 1000 * 1000;
        private ParserOptions parserOptions = new ParserOptions();

        public ParserOptions ParserOptions
        {
            get => parserOptions;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                parserOptions = value;
            }
        }

        public int MaxRequestSize
        {
            get => maxRequestSize;
            set
            {
                if (value < minMaxRequestSize)
                {
                    throw new ArgumentException("The minimum max request size is 1024 bytes.", nameof(value));
                }

                maxRequestSize = value;
            }
        }
    }
}
