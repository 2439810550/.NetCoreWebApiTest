namespace day1.Models
{
    public class Order_item
    {
        /// <summary>
        /// 订单明细Id，主键，自增
        /// </summary>
        public int Id { get; set; }
        public long DishId { get; set; }
        public long OrderId { get; set; }
        /// <summary>
        /// 菜品数量
        /// </summary>
        public int Quantity { get; set; }

        public decimal Price { get; set; }   // 下单时的单价快照

        // 导航属性
        public Order Order { get; set; } = null!;
        public Dish Dish { get; set; } = null!;

    }
}
