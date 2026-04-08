namespace day1.Models
{
    public class Cart
    {
            public long Id { get; set; }
            public long UserId { get; set; }
            public long DishId { get; set; }
            public int Quantity { get; set; }
            public DateTime AddTime { get; set; }

            // 导航属性
            public User User { get; set; } = null!;
            public Dish Dish { get; set; } = null!;
        }
    
}
