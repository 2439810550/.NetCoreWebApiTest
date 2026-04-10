using day1.Models;

namespace day1.Repositories.Order
{
    public interface IOrderRepository
    {
        Task<List<Models.Order>> GetOrders(long userid);
    }
}
