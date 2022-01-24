using HotChocolate;
using HotChocolate.Types;
using System.Linq;
using Template.GraphQL.Middleware.Abstraction;

namespace Template.SystemApis.SqlBdd1.GraphQL
{
    public class Query : IGraphQLMiddlewareQuery
    {
        [UseSelection]
        [UseFiltering]
        public IQueryable<EnteteCommandeAchat> GetEntetesCommandeAchat([Service] OrderContext context)
        {
            return context.EntetesCommandeAchat;
        }

    }
}
