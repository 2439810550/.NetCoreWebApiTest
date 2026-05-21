
using day1.DTOs.CartAndOrderDto;
using day1.DTOs;
using day1.Models;

namespace day1.Services.CartAndOrder
{
    public interface ICartAndOrderServer
    {
        Task<CartsDto> UpdateToCart(long userid, long dishid, int quantity);

        Task<CartsDto> GetCarts(long userid);

        Task ClearCart(long userid);

        Task<OrderDto> PlaceOrder(long userid, string? remark);

        Task<List<OrderDto>> GetUserOrders(long userid);

        Task<List<OrderDto>> GetAllOrders();

        Task<bool> UpdateOrderStatus(long orderId, OrderStatus status);

        Task<bool> CancelOrder(long orderId, long userId);

        Task<PagedResult<OrderDto>> SearchOrders(OrderSearchDto searchDto, long? userId = null);

        Task<OrderStatisticsDto> GetOrderStatistics();
    }
}
