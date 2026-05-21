using System.Security.Claims;
using Azure.Core;
using day1.DTOs.CartAndOrderDto;
using day1.DTOs;
using day1.Repositories.CartAndOrder;
using Microsoft.AspNetCore.Authorization;
using day1.Models;

namespace day1.Services.CartAndOrder
{
    public class CartAndOrderServer:ICartAndOrderServer
    {
        public readonly ICartAndOrderRepository _cartAndOrderRepository;
        public CartAndOrderServer(ICartAndOrderRepository cartAndOrderRepository)
        {
            _cartAndOrderRepository = cartAndOrderRepository;
        }

        public async Task ClearCart(long userid)
        {
             await _cartAndOrderRepository.ClearCart(userid);
        }

        public async Task<CartsDto> GetCarts(long userid)
        {
            return await _cartAndOrderRepository.GetCart(userid);
        }

        public async Task<CartsDto> UpdateToCart(long userid, long dishid, int quantity)
        {
            var cartitem = await _cartAndOrderRepository.GetCartItem(userid, dishid);
            if (quantity <= 0)
            {
                if (cartitem != null)
                {
                    await _cartAndOrderRepository.RemoveFromCart(userid, dishid);
                }
                return await GetCarts(userid);
            }
            if (cartitem == null)
            {
                await _cartAndOrderRepository.AddToCart(userid, dishid, quantity);
            }
            else 
            {
                cartitem.Quantity = quantity;
            }
            await _cartAndOrderRepository.SaveChangesAsync();
            return await GetCarts(userid);
        }
        /// <summary>
        /// �µ�
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public async Task<OrderDto> PlaceOrder(long userid, string? remark)
        {
            var order = await _cartAndOrderRepository.PlaceOrder(userid, remark);
            var orders = await _cartAndOrderRepository.GetOrders(userid);
            return orders.First(o => o.Id == order.Id);
        }

        public async Task<List<OrderDto>> GetUserOrders(long userid)
        {
            return await _cartAndOrderRepository.GetOrders(userid);
        }

        public async Task<List<OrderDto>> GetAllOrders()
        {
            return await _cartAndOrderRepository.GetAllOrders();
        }

        public async Task<bool> UpdateOrderStatus(long orderId, OrderStatus status)
        {
            return await _cartAndOrderRepository.UpdateOrderStatus(orderId, status);
        }

        public async Task<bool> CancelOrder(long orderId, long userId)
        {
            return await _cartAndOrderRepository.CancelOrder(orderId, userId);
        }

        public async Task<PagedResult<OrderDto>> SearchOrders(OrderSearchDto searchDto, long? userId = null)
        {
            return await _cartAndOrderRepository.SearchOrders(searchDto, userId);
        }

        public async Task<OrderStatisticsDto> GetOrderStatistics()
        {
            return await _cartAndOrderRepository.GetOrderStatistics();
        }
    }
}
