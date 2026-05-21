using System.Text.Json;
using System.Text.RegularExpressions;
using day1.Common;
using day1.DYSdk.DYApi;
using day1.DYSdk.DYModels;
using day1.DTOs;

namespace day1.Services
{
    public class DYVideoServer
    {
        private readonly IMediaParserService _mediaParser;
        private readonly IDouYinVideoApiService _xinyewApi;

        private static readonly Regex DouyinUrlRegex = new(
            @"(https?://(?:v\.douyin\.com|www\.douyin\.com|www\.iesdouyin\.com|douyin\.com)[\w\-\./~?=&%#]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public DYVideoServer(IMediaParserService mediaParser, IDouYinVideoApiService xinyewApi)
        {
            _mediaParser = mediaParser;
            _xinyewApi = xinyewApi;
        }

        private static string ExtractDouyinUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var trimmed = input.Trim();

            if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                var spaceIndex = trimmed.IndexOf(' ');
                if (spaceIndex > 0)
                    trimmed = trimmed.Substring(0, spaceIndex);
                return trimmed.TrimEnd('/', ',', '.', ';', '，', '。');
            }

            var match = DouyinUrlRegex.Match(trimmed);
            if (match.Success)
            {
                var url = match.Value.TrimEnd('/', ',', '.', ';', '，', '。');
                return url;
            }

            var parts = trimmed.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (part.Contains("douyin.com", StringComparison.OrdinalIgnoreCase))
                {
                    var clean = part.TrimEnd('/', ',', '.', ';', '，', '。');
                    if (clean.StartsWith("http://") || clean.StartsWith("https://"))
                        return clean;
                    if (clean.StartsWith("v.douyin.com", StringComparison.OrdinalIgnoreCase) ||
                        clean.Contains("douyin.com/"))
                        return "https://" + clean;
                }
            }

            return trimmed;
        }

        public async Task<ApiResponse<DYIRemoveVideoWatermarkDTO>> RemoveWatermarkAsync(string input)
        {
            // 主解析：media-parser
            var mpResult = await _mediaParser.ParseAsync(input);
            if (mpResult.Retcode == 200 && mpResult.Data != null)
            {
                var mpData = mpResult.Data;
                var hasVideo = !string.IsNullOrWhiteSpace(mpData.Video_Url);
                var images = ParseImageList(mpData.Image_List);

                if (hasVideo || images.Count > 0)
                {
                    var data = new DYIRemoveVideoWatermarkDTO
                    {
                        MediaType = hasVideo && images.Count > 0 ? "mixed" :
                                    images.Count > 0 ? "image" : "video",
                        VideoUrl = mpData.Video_Url,
                        PlayUrl = mpData.Video_Url,
                        Description = mpData.Title,
                        AuthorNickname = mpData.Author?.Nickname,
                        AuthorAvatar = mpData.Author?.Avatar,
                        CoverUrl = mpData.Cover_Url,
                        AudioUrl = mpData.Audio_Url,
                        Images = images
                    };
                    return ApiResponse<DYIRemoveVideoWatermarkDTO>.SuccessResponse(data);
                }
            }

            // Fallback：旧接口 xinyew.cn
            var cleanUrl = ExtractDouyinUrl(input);
            var xyResult = await _xinyewApi.RemoveVideoWatermarkAsync(cleanUrl);
            if (xyResult.Code == 200)
            {
                var additionalData = xyResult.Data?.Additional_Data?.FirstOrDefault();
                var data = new DYIRemoveVideoWatermarkDTO
                {
                    MediaType = "video",
                    VideoUrl = xyResult.Data?.Video_Url,
                    PlayUrl = xyResult.Data?.Play_Url,
                    AuthorNickname = additionalData.Nickname,
                    Description = additionalData.Desc,
                    AuthorAvatar = additionalData.Url,
                    CoverUrl = additionalData.Url,
                    Images = new List<MediaImageItem>()
                };
                return ApiResponse<DYIRemoveVideoWatermarkDTO>.SuccessResponse(data);
            }

            return ApiResponse<DYIRemoveVideoWatermarkDTO>.FailResponse(
                mpResult.Retcode != 200 ? mpResult.Retcode : xyResult.Code,
                mpResult.Retcode != 200 ? mpResult.Retdesc : xyResult.Message);
        }

        private static List<MediaImageItem> ParseImageList(JsonElement imageListElement)
        {
            var result = new List<MediaImageItem>();
            if (imageListElement.ValueKind != JsonValueKind.Array)
                return result;

            foreach (var item in imageListElement.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    result.Add(new MediaImageItem { Url = item.GetString() });
                }
                else if (item.ValueKind == JsonValueKind.Object)
                {
                    result.Add(new MediaImageItem
                    {
                        Url = item.TryGetProperty("url", out var u) ? u.GetString() : null,
                        LivePhotoUrl = item.TryGetProperty("live_photo_url", out var l) ? l.GetString() : null
                    });
                }
            }
            return result;
        }
    }
}
