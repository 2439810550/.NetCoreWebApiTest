namespace day1.Models
{
    public class Cart
    {
        /// <summary>
        /// 购物车Id，主键，自增
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 用户Id，外键，关联User表
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 菜品Id，外键，关联Dish表
        /// </summary>
        public long DishId { get; set; }
        /// <summary>
        /// 菜品数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 添加到购物车的时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 导航属性：一个购物车项关联一个用户和一个菜品
        /// </summary>
        // 导航属性
        public User User { get; set; } = null!;
        /// <summary>
        /// 导航属性：一个购物车项关联一个用户和一个菜品
        /// </summary>
        public Dish Dish { get; set; } = null!;
    }
    
}
