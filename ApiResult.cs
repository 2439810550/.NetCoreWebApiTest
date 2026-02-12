namespace day1
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static ApiResult Ok(object data)
        {
            return new ApiResult { Success = true, Data = data };
        }

        public static ApiResult Fail(string message)
        {
            return new ApiResult { Success = false, Message = message };
        }
    }

}
