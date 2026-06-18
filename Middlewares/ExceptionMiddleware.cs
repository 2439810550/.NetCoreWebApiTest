using System.Net;
using System.Text.Json;
using day1.Common;
using day1.Domain;
using Serilog;
namespace day1.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        /// <summary>
        /// 全局异常处理中间件，捕获所有未处理的异常，并根据异常类型返回相应的 HTTP 状态码和错误信息。
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "业务异常");
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                httpContext.Response.ContentType = "application/json";
                var response = new ApiResponse<object>
                {
                    Success = false,
                    Code = ex.Code,
                    Message = ex.Message
                };
                await httpContext.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "系统异常");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";
                var response = new ApiResponse<object>
                {
                    Success = false,
                    // 生产环境不暴露异常详情，仅返回通用错误信息
                    Message = "服务器内部错误，请稍后重试",
                    Code=500
                };
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
