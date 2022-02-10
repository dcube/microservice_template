using Refit;
using System.Threading.Tasks;
using Template.GraphQL;

namespace Template.ProcessApis.Api1.Services
{
    public interface IOrderSystemApiFunction
    {
        [Post("/api/graphQL")]
        Task<T> Query<T>([Body] GraphQLRequest request);
    }
}
