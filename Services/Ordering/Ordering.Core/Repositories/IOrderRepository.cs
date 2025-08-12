using Ordering.Core.Entities;

namespace Ordering.Core.Repositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
    }
}
