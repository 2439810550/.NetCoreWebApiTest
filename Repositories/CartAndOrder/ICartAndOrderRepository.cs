using day1.DTOs.CartAndOrderDto;
using day1.DTOs;
using day1.Models;

namespace day1.Repositories.CartAndOrder
{
    public interface ICartAndOrderRepository
    {
        Task<List<OrderDto>> GetOrders(long userid);

        Task<List<OrderDto>> GetAllOrders();

        Task<CartsDto> GetCart(long userid);

        Task AddToCart(long userid, long dishid, int quantity);

        Task RemoveFromCart(long userid, long dishid);

        Task ClearCart(long userid);

        Task<Order> PlaceOrder(long userid, string? remark);

        Task<Cart> GetCartItem(long userid, long dishid);

        Task<int> SaveChangesAsync();

        Task<bool> UpdateOrderStatus(long orderId, OrderStatus status);

        Task<bool> CancelOrder(long orderId, long userId);

        Task<PagedResult<OrderDto>> SearchOrders(OrderSearchDto searchDto, long? userId = null);

        Task<OrderStatisticsDto> GetOrderStatistics();
    }
}
