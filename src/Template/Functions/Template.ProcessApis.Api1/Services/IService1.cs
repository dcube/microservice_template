using System.Threading.Tasks;
using Template.Domain;
using Template.ProcessApis.Api1.ResponseModels;
using Template.Sqlbdd1.Dto;

namespace Template.ProcessApis.Api1.Services
{
    public interface IService1
    {
        Task<OrderResponse> GetAsync(int id);
        Result Save(OrderDto Order);
    }
}
