using System;
using System.Threading.Tasks;
using Template.Domain;
using Template.GraphQL;
using Template.GraphQL.Query;
using Template.ProcessApis.Api1.RequestModel;
using Template.ProcessApis.Api1.ResponseModels;
using Template.Sqlbdd1.Dto;

namespace Template.ProcessApis.Api1.Services
{
    internal class Service1 : IService1
    {
        private readonly IOrderSystemApiFunction _orderSystemApiFunction;

        public Service1(IOrderSystemApiFunction orderSystemApiFunction)
        {
            _orderSystemApiFunction = orderSystemApiFunction ?? throw new ApplicationException(nameof(orderSystemApiFunction));
        }

        public async Task<OrderResponse> GetAsync(int id)
        {
            var builder = new GraphQLQueryBuilder()
                .AddVariable("id", GraphQLParameterType.INT, id)
                .AddQuery(new GraphQLQueryObject<OrderDto>("orderDto")
                    .WithArguments(new
                    {
                        Id = "id"
                    })
                    .AddEveryFields()
                );

            return await _orderSystemApiFunction.Query<OrderResponse>(new GraphQLRequest()
            {
                Query = builder.Query,
                Variables = builder.Variables
            });
        }

        public Result Save(OrderRequest Order)
        {
            return new Result() { IsSuccess = true};
        }
    }
}
