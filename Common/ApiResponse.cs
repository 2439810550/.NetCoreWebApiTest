namespace day1.Common
{
    public class ApiResponse<T>
    {
        public bool Success {get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data,string message="成功")
        {
            return new ApiResponse<T> { Success = true,Message=message, Data = data };
        }
    }
}
