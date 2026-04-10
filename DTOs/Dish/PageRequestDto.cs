namespace day1.DTOs.Dish
{
    public class PageRequestDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? KeyWord { get; set; }
    }
}
