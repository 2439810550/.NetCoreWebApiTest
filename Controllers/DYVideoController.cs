using day1.Common;
using System;
using System.Net.Http.Headers;
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
        private readonly IHttpClientFactory _httpClientFactory;

        public DYVideoController(Services.DYVideoServer dyVideoServer, IHttpClientFactory httpClientFactory)
        {
            _dyVideoServer = dyVideoServer;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> RemoveWatermark(string videoUrl)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                return BadRequest(ApiResponse<DYIRemoveVideoWatermarkDTO>.FailResponse(400, "链接不能为空"));
            }
            return Ok(await _dyVideoServer.RemoveWatermarkAsync(videoUrl));
        }

        [HttpGet("proxy")]
        public async Task ProxyVideo(string url, CancellationToken cancellationToken)
        {
            var decodedUrl = Uri.UnescapeDataString(url);

            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            // 根据目标域名动态设置 Referer，避免被 CDN 拒绝
            var targetHost = new Uri(decodedUrl).Host;
            if (targetHost.Contains("douyin.com") || targetHost.Contains("zjcdn.com") ||
                targetHost.Contains("snssdk.com") || targetHost.Contains("douyinstatic.com"))
            {
                client.DefaultRequestHeaders.Referrer = new Uri("https://www.douyin.com/");
            }

            client.Timeout = TimeSpan.FromSeconds(60);

            var response = await client.GetAsync(decodedUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            Response.StatusCode = (int)response.StatusCode;
            Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "video/mp4";

            var contentLength = response.Content.Headers.ContentLength;
            if (contentLength.HasValue)
                Response.ContentLength = contentLength.Value;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await stream.CopyToAsync(Response.Body, cancellationToken);
        }
    }
}
