using System.Collections.Generic;
using System.Threading.Tasks;
using Template.Sqlbdd1.Dto;

namespace Template.SystemApis.SqlBdd1.Interfaces
{
    public interface IOrderRepository : IRepository
    {
        Task<int> UpdateOrderAsync(OrderDto entetCommandeAchat);

        Task<int> InsertOrderAsync(OrderDto entetCommandeAchat);

        Task<OrderDto> GetOrderAsync(int id);
    }
}
