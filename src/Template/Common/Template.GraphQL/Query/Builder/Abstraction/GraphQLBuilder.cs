using System.Text;

namespace Template.GraphQL.Query
{
    public abstract class GraphQLBuilder
    {
        protected StringBuilder builder;

        protected Dictionary<string, GraphQLParameter> parameters;

        public GraphQLBuilder()
        {
            this.builder = new StringBuilder();
            this.parameters = new Dictionary<string, GraphQLParameter>();
        }

        protected string GetTabulation(int tabCount)
        {
            return new string('\t', tabCount);
        }
    }
}
