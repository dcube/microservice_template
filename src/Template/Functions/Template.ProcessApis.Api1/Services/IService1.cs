using System.Threading.Tasks;
using Template.Domain;
using Template.ProcessApis.Api1.RequestModel;
using Template.ProcessApis.Api1.ResponseModels;

namespace Template.ProcessApis.Api1.Services
{
    internal interface IService1
    {
        Task<OrderResponse> GetAsync(int id);
        Result Save(OrderRequest Order);
    }
}
