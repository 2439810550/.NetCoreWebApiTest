using day1.Common;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using day1.DTOs;

namespace day1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DYVideoController : ControllerBase
    {
        public readonly Services.DYVideoServer _dyVideoServer;
        public DYVideoController(Services.DYVideoServer dyVideoServer)
        {
            _dyVideoServer = dyVideoServer;
        }
        [HttpGet]
        public async Task<IActionResult> RemoveWatermark(string videoUrl)
        {
            // 简单参数校验
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                return BadRequest(ApiResponse<DYIRemoveVideoWatermarkDTO>.FailResponse(400, "链接不能为空"));
            }
            return Ok(await _dyVideoServer.RemoveWatermarkAsync(videoUrl));

        }
    }
}
