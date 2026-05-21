using day1.Data;
using day1.DTOs.CartAndOrderDto;
using day1.DTOs;
using day1.Models;
using Microsoft.EntityFrameworkCore;

namespace day1.Repositories.CartAndOrder
{
    public class CartAndOrderRepository : ICartAndOrderRepository
    {
        public readonly AppDbContext _context;
        public CartAndOrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddToCart(long userid, long dishid, int quantity)
        {
            await _context.Carts.AddAsync(new Cart { UserId = userid, DishId = dishid, Quantity = quantity, AddTime = DateTime.UtcNow });
        }

        public async Task ClearCart(long userid)
        {
            await _context.Carts.Where(c => c.UserId == userid).ExecuteDeleteAsync();
        }

        public async Task<CartsDto> GetCart(long userid)
        {
            var cartsitemlist = await _context.Carts
                .Where(c => c.UserId == userid)
                .Include(c => c.Dish)
                .Select(c => new CartItemDto 
                { 
                    Id = c.Id, 
                    DishId = c.DishId, 
                    UserId = c.UserId, 
                    AddTime = c.AddTime, 
                    Quantity = c.Quantity, 
                    DishName = c.Dish.Name, 
                    DishPrice = c.Dish.Price, 
                    ImageUrl = c.Dish.ImageUrl 
                })
                .ToListAsync();
            return new CartsDto { Items = cartsitemlist };
        }

        public async Task<Cart> GetCartItem(long userid, long dishid)
        {
            return await _context.Carts.Include(c => c.Dish).FirstOrDefaultAsync(c => c.UserId == userid && c.DishId == dishid);
        }

        public async Task<List<OrderDto>> GetOrders(long userid)
        {
            return await _context.Orders
                .Where(o => o.UserId == userid)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .OrderByDescending(o => o.OrderCreatTime)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNo = o.Order_No,
                    OrderCreatTime = o.OrderCreatTime,
                    OrderPrice = o.Order_Price,
                    Remark = o.Remark,
                    Status = o.Status,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        DishId = oi.DishId,
                        DishName = oi.Dish.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        ImageUrl = oi.Dish.ImageUrl
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<Order> PlaceOrder(long userid, string? remark)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == userid)
                    .Include(c => c.Dish)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    throw new InvalidOperationException("购物车为空，无法下单");
                }

                var order = new Order
                {
                    UserId = userid,
                    Order_No = GenerateOrderNo(),
                    OrderCreatTime = DateTime.UtcNow,
                    Remark = remark,
                    Order_Price = cartItems.Sum(c => c.Quantity * c.Dish.Price),
                    OrderItems = cartItems.Select(c => new Order_item
                    {
                        DishId = c.DishId,
                        Quantity = c.Quantity,
                        Price = c.Dish.Price
                    }).ToList()
                };

                await _context.Orders.AddAsync(order);
                await _context.Carts.Where(c => c.UserId == userid).ExecuteDeleteAsync();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveFromCart(long userid, long dishid)
        {
            await _context.Carts.Where(c => c.UserId == userid && c.DishId == dishid).ExecuteDeleteAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private string GenerateOrderNo()
        {
            return $"ORD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        public async Task<List<OrderDto>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .OrderByDescending(o => o.OrderCreatTime)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNo = o.Order_No,
                    OrderCreatTime = o.OrderCreatTime,
                    OrderPrice = o.Order_Price,
                    Remark = o.Remark,
                    Status = o.Status,
                    UserName = o.User.UserName,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        DishId = oi.DishId,
                        DishName = oi.Dish.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        ImageUrl = oi.Dish.ImageUrl
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatus(long orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }
            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrder(long orderId, long userId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if (order == null || order.Status != OrderStatus.Pending)
            {
                return false;
            }
            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<OrderDto>> SearchOrders(OrderSearchDto searchDto, long? userId = null)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .AsQueryable();

            // 如果指定了用户ID，只查询该用户的订单
            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            // 关键词搜索（订单号或用户名）
            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.Trim();
                query = query.Where(o => o.Order_No.Contains(keyword) || o.User.UserName.Contains(keyword));
            }

            // 状态筛选
            if (searchDto.Status.HasValue)
            {
                query = query.Where(o => o.Status == searchDto.Status.Value);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderCreatTime)
                .Skip((searchDto.PageIndex - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNo = o.Order_No,
                    OrderCreatTime = o.OrderCreatTime,
                    OrderPrice = o.Order_Price,
                    Remark = o.Remark,
                    Status = o.Status,
                    UserName = o.User.UserName,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        DishId = oi.DishId,
                        DishName = oi.Dish.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        ImageUrl = oi.Dish.ImageUrl
                    }).ToList()
                })
                .ToListAsync();

            return new PagedResult<OrderDto>
            {
                Items = orders,
                TotalCount = totalCount,
                PageIndex = searchDto.PageIndex,
                PageSize = searchDto.PageSize
            };
        }

        public async Task<OrderStatisticsDto> GetOrderStatistics()
        {
            var today = DateTime.Today;
            var todayOrders = await _context.Orders
                .Where(o => o.OrderCreatTime >= today)
                .ToListAsync();

            var allOrders = await _context.Orders.ToListAsync();

            return new OrderStatisticsDto
            {
                TodayOrderCount = todayOrders.Count,
                TodayRevenue = todayOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.Order_Price),
                TotalOrderCount = allOrders.Count,
                TotalRevenue = allOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.Order_Price),
                PendingOrderCount = allOrders.Count(o => o.Status == OrderStatus.Pending),
                CompletedOrderCount = allOrders.Count(o => o.Status == OrderStatus.Completed)
            };
        }
    }
}
