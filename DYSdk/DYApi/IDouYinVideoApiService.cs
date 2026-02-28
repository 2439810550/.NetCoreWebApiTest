using System.Text.Json;
using day1.DYSdk.DYModels;

namespace day1.DYSdk.DYApi
{
    public interface IDouYinVideoApiService
    {
        Task<DYVideoRemoveWatermarkApiResponse> RemoveVideoWatermarkAsync(string videoUrl);
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
                //return System.Text.Json.JsonSerializer.Deserialize<DYVideoRemoveWatermarkApiResponse>(content);
                return JsonSerializer.Deserialize<DYVideoRemoveWatermarkApiResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                // 记录日志（可选）
                return new DYVideoRemoveWatermarkApiResponse { Code = -1, Message = $"网络请求失败: {ex.Message}" };
            }
            catch (System.Text.Json.JsonException ex)
            {
                return new DYVideoRemoveWatermarkApiResponse { Code = -1, Message = $"数据解析失败: {ex.Message}" };
            }

        }
    }
}
