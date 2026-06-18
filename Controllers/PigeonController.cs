using day1.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace day1.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class PigeonController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<PigeonController> _logger;

    // 上传保存目录（相对于 wwwroot）
    private const string UploadDir = "uploads/pigeon";

    public PigeonController(IWebHostEnvironment env, ILogger<PigeonController> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>
    /// 获取上传文件的物理保存目录
    /// </summary>
    private string GetUploadPath()
    {
        var path = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), UploadDir);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// 从文件名提取 ID（文件名格式: {guid}_original.ext 或 {guid}_processed.ext）
    /// </summary>
    private static string? ExtractIdFromFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        if (name.EndsWith("_original"))
            return name.Substring(0, name.Length - "_original".Length);
        if (name.EndsWith("_processed"))
            return name.Substring(0, name.Length - "_processed".Length);
        return null;
    }

    /// <summary>
    /// 上传原始图片和合成视频
    /// </summary>
    [HttpPost("upload")]
    [RequestSizeLimit(30_000_000)] // 30MB 总大小限制
    public async Task<IActionResult> Upload(IFormFile originalImage, IFormFile processedVideo)
    {
        // 参数验证
        if (originalImage == null || originalImage.Length == 0)
            return BadRequest(ApiResponse<object>.FailResponse(400, "缺少 originalImage 文件"));
        if (processedVideo == null || processedVideo.Length == 0)
            return BadRequest(ApiResponse<object>.FailResponse(400, "缺少 processedVideo 文件"));

        // 文件大小验证
        if (originalImage.Length > 5_000_000)
            return BadRequest(ApiResponse<object>.FailResponse(400, "originalImage 文件超过 5MB 限制"));
        if (processedVideo.Length > 20_000_000)
            return BadRequest(ApiResponse<object>.FailResponse(400, "processedVideo 文件超过 20MB 限制"));

        // 文件类型验证
        var allowedImageTypes = new[] { ".png", ".jpg", ".jpeg" };
        var imageExt = Path.GetExtension(originalImage.FileName).ToLowerInvariant();
        if (!allowedImageTypes.Contains(imageExt))
            return BadRequest(ApiResponse<object>.FailResponse(400, "originalImage 仅支持 PNG/JPG 格式"));

        var videoExt = Path.GetExtension(processedVideo.FileName).ToLowerInvariant();
        if (videoExt != ".webm")
            return BadRequest(ApiResponse<object>.FailResponse(400, "processedVideo 仅支持 WebM 格式"));

        var id = Guid.NewGuid().ToString("N");
        var uploadPath = GetUploadPath();

        // 保存原始图片
        var originalFileName = $"{id}_original{imageExt}";
        var originalPath = Path.Combine(uploadPath, originalFileName);
        try
        {
            await using (var stream = new FileStream(originalPath, FileMode.Create))
            {
                await originalImage.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存原始图片失败: {Path}", originalPath);
            return StatusCode(500, ApiResponse<object>.FailResponse(500, "保存原始图片失败，请重试"));
        }

        // 保存合成视频
        var videoFileName = $"{id}_processed.webm";
        var videoPath = Path.Combine(uploadPath, videoFileName);
        try
        {
            await using (var stream = new FileStream(videoPath, FileMode.Create))
            {
                await processedVideo.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存合成视频失败: {Path}", videoPath);
            return StatusCode(500, ApiResponse<object>.FailResponse(500, "保存合成视频失败，请重试"));
        }

        _logger.LogInformation("鸽子摇上传成功: {Id}, 原图: {Original}, 视频: {Video}",
            id, originalFileName, videoFileName);

        return Ok(ApiResponse<object>.SuccessResponse(new
        {
            id,
            originalUrl = $"/{UploadDir}/{originalFileName}",
            processedUrl = $"/{UploadDir}/{videoFileName}",
            createdAt = DateTime.UtcNow
        }));
    }

    /// <summary>
    /// 获取上传历史（按时间倒序分页）
    /// </summary>
    [HttpGet("gallery")]
    public IActionResult GetGallery([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var uploadPath = GetUploadPath();
        if (!Directory.Exists(uploadPath))
            return Ok(ApiResponse<object>.SuccessResponse(new { items = Array.Empty<object>(), total = 0, page, pageSize }));

        // 扫描目录中的文件，按 _original / _processed 配对
        var files = new DirectoryInfo(uploadPath).GetFiles()
            .OrderByDescending(f => f.CreationTimeUtc)
            .ToList();

        // 按 ID 分组（同一 ID 的 original + processed 为一组）
        var groups = files
            .Select(f =>
            {
                var id = ExtractIdFromFileName(f.Name);
                var isOriginal = Path.GetFileNameWithoutExtension(f.Name).EndsWith("_original");
                return new { File = f, Id = id, IsOriginal = isOriginal };
            })
            .Where(x => x.Id != null)
            .GroupBy(x => x.Id)
            .OrderByDescending(g => g.Max(x => x.File.CreationTimeUtc))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g =>
            {
                var original = g.FirstOrDefault(x => x.IsOriginal);
                var processed = g.FirstOrDefault(x => !x.IsOriginal);
                return new
                {
                    id = g.Key,
                    originalUrl = original != null ? $"/{UploadDir}/{original.File.Name}" : null,
                    processedUrl = processed != null ? $"/{UploadDir}/{processed.File.Name}" : null,
                    createdAt = g.Max(x => x.File.CreationTimeUtc)
                };
            })
            .ToList();

        var total = new DirectoryInfo(uploadPath).GetFiles()
            .Select(f => ExtractIdFromFileName(f.Name))
            .Where(id => id != null)
            .Distinct()
            .Count();

        return Ok(ApiResponse<object>.SuccessResponse(new { items = groups, total, page, pageSize }));
    }
}
