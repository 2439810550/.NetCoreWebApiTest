using day1.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace day1.Filter
{
    public class ApiResponseFilter : IResultFilter
    {
        /// <summary>
        /// 在结果执行后调用，可以用于修改响应数据或添加额外的处理逻辑。
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnResultExecuted(ResultExecutedContext context)
        {
            
        }

        /// <summary>
        /// 在结果执行前调用，可以用于修改即将返回的结果或添加额外的处理逻辑。
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {

                if (objectResult.Value is ApiResponse<object>)
                    return;
                var apiResponse=ApiResponse<object>.SuccessResponse(objectResult.Value);
                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
        }
    }
}
