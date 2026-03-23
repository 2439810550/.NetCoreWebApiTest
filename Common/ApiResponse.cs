namespace day1.Common
{
    public class ApiResponse<T>
    {
        public bool Success {get; set; }
        public string Message { get; set; }

        public int? Code { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "成功")
        {
            return new ApiResponse<T> { Success = true,Message=message, Data = data };
        }

        public static ApiResponse<T> SuccessResponse(T data, int? code, string message = "成功")
        {
            return new ApiResponse<T>
            {
                Success= true,
                Code = code,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> FailResponse(int code, string message)
        {
            return new ApiResponse<T> { Success = false, Message = message,Code=code,Data=default };
        }
    }
}
