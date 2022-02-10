using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template.Sqlbdd1.Dto;
using Template.SystemApis.SqlBdd1.Interfaces;
using Template.SystemApis.SqlBdd1.Repositories.Context;

namespace Template.SystemApis.SqlBdd1.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SqlBdd1Context _context;

        public OrderRepository(SqlBdd1Context context) 
        {
            _context = context ?? throw new ApplicationException(nameof(context));
        }

        public async Task<int> UpdateOrderAsync(OrderDto order)
        {
            OrderDto orderToUpdate = _context.Order
                .FirstOrDefault(o => o.Id == order.Id);

            if (orderToUpdate == null)
            {
                throw new ApplicationException($"Order {order.Id} non trouvée");
            }

            _context.Order.Update(order);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> InsertOrderAsync(OrderDto order)
        {
            await _context.Order.AddAsync(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }

        public async Task<OrderDto> GetOrderAsync(int id)
        {
            return await _context.Order
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
