namespace day1.DTOs.Dish
{
    public class DishDTO
    {
        public long? ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
