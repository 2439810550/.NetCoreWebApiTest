namespace day1.Models
{
    public class Order
    {
        /// <summary>
        /// 订单Id，主键，自增
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 订单编号，唯一
        /// </summary>
        public string Order_No { get; set; }
        /// <summary>
        /// 下单用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime OrderCreatTime { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string? Remark { get;set;}
        /// <summary>
        /// 订单总价
        /// </summary>
        public decimal Order_Price { get; set; }

        // 导航属性：下单用户
        public User User { get; set; } = null!;

        // 一个订单包含多个明细
        public ICollection<Order_item> OrderItems { get; set; } = new List<Order_item>();

    }
}
