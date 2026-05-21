using System.Text.Json;
using day1.DYSdk.DYModels;

namespace day1.DYSdk.DYApi
{
    public interface IDouYinVideoApiService
    {
        Task<DYVideoRemoveWatermarkApiResponse> RemoveVideoWatermarkAsync(string videoUrl);
    }

    public interface IMediaParserService
    {
        Task<MediaParserResponse> ParseAsync(string text);
    }

    public class DouYinVideoApiService : IDouYinVideoApiService
    {
        private readonly HttpClient _httpClient;
        public DouYinVideoApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<DYVideoRemoveWatermarkApiResponse> RemoveVideoWatermarkAsync(string videoUrl)
        {
            var requestUrl = $"https://api.xinyew.cn/api/douyinjx?url={Uri.EscapeDataString(videoUrl)}";
            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DYVideoRemoveWatermarkApiResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                return new DYVideoRemoveWatermarkApiResponse { Code = -1, Message = $"网络请求失败: {ex.Message}" };
            }
            catch (System.Text.Json.JsonException ex)
            {
                return new DYVideoRemoveWatermarkApiResponse { Code = -1, Message = $"数据解析失败: {ex.Message}" };
            }

        }
    }

    public class MediaParserService : IMediaParserService
    {
        private readonly HttpClient _httpClient;
        public MediaParserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<MediaParserResponse> ParseAsync(string text)
        {
            var payload = JsonSerializer.Serialize(new { text });
            try
            {
                var response = await _httpClient.PostAsync("/api/parse",
                    new StringContent(payload, System.Text.Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MediaParserResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                return new MediaParserResponse { Retcode = -1, Retdesc = $"请求失败: {ex.Message}" };
            }
            catch (JsonException ex)
            {
                return new MediaParserResponse { Retcode = -1, Retdesc = $"解析失败: {ex.Message}" };
            }
        }
    }
}
