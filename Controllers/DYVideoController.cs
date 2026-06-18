using day1.Common;
using System;
using System.Net.Http;
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
        private readonly IConfiguration _configuration;

        public DYVideoController(Services.DYVideoServer dyVideoServer, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _dyVideoServer = dyVideoServer;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        /// <summary>
        /// 抖音/多平台视频去水印解析
        /// </summary>
        /// <param name="videoUrl">视频分享链接或文本</param>
        /// <param name="proxyUrl">可选：代理地址（如 http://127.0.0.1:7897），用于解析外网平台</param>
        [HttpGet]
        public async Task<IActionResult> RemoveWatermark(string videoUrl, string? proxyUrl = null)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                return BadRequest(ApiResponse<DYIRemoveVideoWatermarkDTO>.FailResponse(400, "链接不能为空"));
            }
            return Ok(await _dyVideoServer.RemoveWatermarkAsync(videoUrl, proxyUrl));
        }

        /// <summary>
        /// 视频/图片代理下载（解决跨域/防盗链，支持强制下载）
        /// 核心改进：手动跟随重定向，每次跳转都带上正确的 Referer，
        /// 解决 .NET HttpClient 自动重定向时会剥离 Referer 导致 CDN 拒绝的问题。
        /// </summary>
        [HttpGet("proxy")]
        public async Task ProxyVideo(string url, bool download = false, string? filename = null, CancellationToken cancellationToken = default)
        {
            var decodedUrl = Uri.UnescapeDataString(url);

            // 处理相对路径：如果 URL 不是完整 HTTP 链接，说明是 media-parser 本地文件
            if (!decodedUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !decodedUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                var mpBase = _configuration["MediaParser:BaseUrl"] ?? "http://127.0.0.1:8051";
                decodedUrl = mpBase.TrimEnd('/') + "/" + decodedUrl.TrimStart('/');
            }

            var client = _httpClientFactory.CreateClient("VideoProxy");

            // 手动跟随重定向（最多 5 次），每次 hop 都设置正确的 Referer
            const int maxRedirects = 5;
            HttpResponseMessage? response = null;
            var currentUrl = decodedUrl;
            var lastException = (Exception?)null;

            for (int hop = 0; hop < maxRedirects; hop++)
            {
                // 释放上一跳的响应（除了最终成功那个）
                response?.Dispose();
                response = null;

                var request = new HttpRequestMessage(HttpMethod.Get, currentUrl);

                // 每次请求都设置对应的 Referer（平台防盗链核心）
                request.Headers.Referrer = GetRefererForUrl(currentUrl);

                try
                {
                    response = await client.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                    // 3xx 重定向 → 继续下一跳
                    var statusCode = (int)response.StatusCode;
                    if (statusCode >= 300 && statusCode < 400)
                    {
                        var location = response.Headers.Location?.ToString();
                        if (string.IsNullOrWhiteSpace(location))
                        {
                            lastException = new InvalidOperationException(
                                $"服务器返回 {statusCode} 但没有 Location 头");
                            break;
                        }
                        // 处理相对路径重定向
                        if (!location.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                            !location.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        {
                            location = new Uri(new Uri(currentUrl), location).ToString();
                        }
                        currentUrl = location;
                        continue;
                    }

                    response.EnsureSuccessStatusCode();
                    break; // 成功
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    break;
                }
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                Response.StatusCode = 502;
                Response.ContentType = "application/json";
                var errMsg = lastException?.Message ?? "未知错误";
                await Response.WriteAsync(
                    $"{{\"success\":false,\"message\":\"视频源请求失败: {errMsg}\"}}", cancellationToken);
                return;
            }

            Response.StatusCode = (int)response.StatusCode;

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "video/mp4";
            Response.ContentType = contentType;

            var contentLength = response.Content.Headers.ContentLength;
            if (contentLength.HasValue)
                Response.ContentLength = contentLength.Value;

            if (download)
            {
                string downloadFilename;
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    downloadFilename = filename;
                }
                else
                {
                    var ext = contentType switch
                    {
                        "video/mp4" => ".mp4",
                        "video/webm" => ".webm",
                        "video/quicktime" => ".mov",
                        "video/mp2t" or "video/vnd.dlna.mpeg-tts" => ".ts",
                        "application/vnd.apple.mpegurl" or "application/x-mpegurl" => ".m3u8",
                        "audio/mpeg" => ".mp3",
                        "audio/mp4" or "audio/x-m4a" => ".m4a",
                        "image/jpeg" => ".jpg",
                        "image/png" => ".png",
                        "image/webp" => ".webp",
                        "image/gif" => ".gif",
                        _ => ".mp4"
                    };
                    downloadFilename = $"media_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{ext}";
                }

                var cd = new ContentDispositionHeaderValue("attachment");
                var encodedName = Uri.EscapeDataString(downloadFilename);
                cd.FileName = encodedName;
                cd.FileNameStar = encodedName;
                Response.Headers.ContentDisposition = cd.ToString();
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await stream.CopyToAsync(Response.Body, cancellationToken);
        }

        /// <summary>
        /// 根据目标域名返回平台对应的 Referer，用于绕过 CDN 防盗链
        /// </summary>
        private static Uri? GetRefererForUrl(string url)
        {
            try
            {
                var host = new Uri(url).Host;
                if (host.Contains("douyin.com") || host.Contains("douyinvod.com") ||
                    host.Contains("douyinpic.com") || host.Contains("zjcdn.com") ||
                    host.Contains("snssdk.com") || host.Contains("douyinstatic.com") ||
                    host.Contains("ixigua.com"))
                    return new Uri("https://www.douyin.com/");
                if (host.Contains("bilibili.com") || host.Contains("bilivideo.com") ||
                    host.Contains("hdslb.com"))
                    return new Uri("https://www.bilibili.com/");
                if (host.Contains("kuaishou.com") || host.Contains("yximgs.com"))
                    return new Uri("https://www.kuaishou.com/");
                if (host.Contains("xiaohongshu.com") || host.Contains("xhscdn.com"))
                    return new Uri("https://www.xiaohongshu.com/");
                if (host.Contains("weibo.cn") || host.Contains("weibo.com") ||
                    host.Contains("sinaimg.cn"))
                    return new Uri("https://weibo.com/");
                if (host.Contains("zhihu.com") || host.Contains("zhimg.com"))
                    return new Uri("https://www.zhihu.com/");
            }
            catch { }
            return null;
        }
    }
}
