namespace day1.DTOs
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }=new List<T>();
        /// <summary>
        /// 总记录数，必须大于或等于0，否则会导致除以0错误
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 当前页码，必须大于0，否则会导致除以0错误
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页显示的记录数，必须大于0，否则会导致除以0错误
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 计算总页数，向上取整
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
