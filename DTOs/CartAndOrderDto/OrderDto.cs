using day1.Models;

namespace day1.DTOs.CartAndOrderDto
{
    public class PlaceOrderDto
    {
        public string? Remark { get; set; }
    }

    public class OrderDto
    {
        public long Id { get; set; }
        public string OrderNo { get; set; } = null!;
        public DateTime OrderCreatTime { get; set; }
        public decimal OrderPrice { get; set; }
        public string? Remark { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusText => GetStatusText(Status);
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public string? UserName { get; set; }

        private static string GetStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "待支付",
                OrderStatus.Paid => "已支付",
                OrderStatus.Preparing => "准备中",
                OrderStatus.Delivering => "配送中",
                OrderStatus.Completed => "已完成",
                OrderStatus.Cancelled => "已取消",
                _ => "未知"
            };
        }
    }

    public class OrderItemDto
    {
        public long DishId { get; set; }
        public string DishName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price;
        public string? ImageUrl { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public long OrderId { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class OrderSearchDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
        public OrderStatus? Status { get; set; }
    }

    public class OrderStatisticsDto
    {
        public int TodayOrderCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalOrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrderCount { get; set; }
        public int CompletedOrderCount { get; set; }
    }
}
