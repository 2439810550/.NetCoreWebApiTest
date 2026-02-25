using System.Net;
using System.Text.Json;
using Serilog;
namespace day1.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"全局异常");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";
                var response = new 
                {    success=false,
                     message = ex.Message
                };
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
