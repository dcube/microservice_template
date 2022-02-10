using HotChocolate;
using HotChocolate.Types;
using System.Linq;
using Template.GraphQL.Middleware.Abstraction;
using Template.Sqlbdd1.Dto;
using Template.SystemApis.SqlBdd1.Repositories.Context;

namespace Template.SystemApis.SqlBdd1.GraphQL
{
    public class Query : IGraphQLMiddlewareQuery
    {
        [UseSelection]
        [UseFiltering]
        public IQueryable<OrderDto> GetEntetesCommandeAchat([Service] SqlBdd1Context context)
        {
            return context.Order;
        }

    }
}
