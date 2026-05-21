namespace day1.DTOs.CartAndOrderDto
{
    public class CartsDto
    {
        /// <summary>
        /// 购物车中的所有条目
        /// </summary>
        public List<CartItemDto> Items { get; set; } = new();

        /// <summary>
        /// 购物车中商品的总数量（所有条目 Quantity 之和）
        /// </summary>
        public int TotalQuantity => Items?.Sum(i => i.Quantity) ?? 0;

        /// <summary>
        /// 购物车中商品的总金额（每个条目 数量×单价 之和）
        /// </summary>
        public decimal TotalPrice => Items?.Sum(i => i.TotalPrice) ?? 0;

    }

    public class CartItemDto
    {
        public long? Id { get; set; }
        public long UserId { get; set; }
        public long DishId { get; set; }
        public string DishName { get; set; } = null!;
        public decimal DishPrice { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// 该条目小计（Quantity * UnitPrice）
        /// </summary>
        public decimal TotalPrice => Quantity * DishPrice;

        /// <summary>
        /// 菜品图片URL（可选）
        /// </summary>
        public string? ImageUrl { get; set; }
        public DateTime AddTime { get; set; }
    }

}
