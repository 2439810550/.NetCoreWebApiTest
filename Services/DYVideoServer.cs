using day1.Common;
using day1.DYSdk.DYApi;
using day1.DYSdk.DYModels;
using day1.DTOs;

namespace day1.Services
{
    public class DYVideoServer
    {
        public readonly IDouYinVideoApiService _douYinVideoApiService;
        public DYVideoServer(IDouYinVideoApiService douYinVideoApiService)
        {
            _douYinVideoApiService = douYinVideoApiService;
        }
        /// <summary>
        /// 调用抖音视频去水印接口，返回去水印后的视频链接和相关信息
        /// </summary>
        /// <param name="videoUrl"></param>
        /// <returns></returns>
        public async Task<ApiResponse<DYIRemoveVideoWatermarkDTO>> RemoveWatermarkAsync(string videoUrl)
        {
            var apiResponse = await _douYinVideoApiService.RemoveVideoWatermarkAsync(videoUrl);
            if (apiResponse.Code == 200)
            {
                var additionalData = apiResponse.Data?.Additional_Data?.FirstOrDefault();
                var data = new DYIRemoveVideoWatermarkDTO
                {
                    VideoUrl = apiResponse.Data?.Video_Url,
                    PlayUrl = apiResponse.Data?.Play_Url,
                    AuthorNickname = additionalData.Nickname,
                    Description=additionalData.Desc,
                    AuthorAvatar = additionalData.Url
                };
                return ApiResponse<DYIRemoveVideoWatermarkDTO>.SuccessResponse(data);
            }
            else
            {
                return ApiResponse<DYIRemoveVideoWatermarkDTO>.FailResponse(apiResponse.Code, apiResponse.Message);
            }
        }
    }
}
