namespace day1.Models
{
    public class Dish
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// 一个菜品可以出现在多个订单明细中
        /// </summary>
        public ICollection<Order_item> OrderItems { get; set; } = new List<Order_item>();
        /// <summary>
        /// 一个菜品可以出现在多个购物车中
        /// </summary>
        public ICollection<Cart> Carts { get; set; }=new List<Cart>();

    }
}
