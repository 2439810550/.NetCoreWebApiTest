using System.Text.Json.Serialization;

namespace day1.DYSdk.DYModels
{
    public class DYVideoRemoveWatermarkApiResponse
    {
        /// <summary>
        /// 状态码，200表示成功，非200表示失败
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 响应信息，成功时返回 "解析成功"
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 成功响应数据内容
        /// </summary>
        public DYVideoRemoveWatermarkData Data { get; set; }
    }

    public class DYVideoRemoveWatermarkData
    {
        /// <summary>
        /// 	视频播放地址
        /// </summary>
        public string Play_Url { get; set; }
        /// <summary>
        /// 	视频文件实际链接地址
        /// </summary>
        public string Video_Url { get; set; }
        /// <summary>
        /// 	解析时间（单位：毫秒）
        /// </summary>
        public string Parse_time { get; set; }
        /// <summary>
        /// 	JSON 数据中提取的附加信息（视频详情等）
        /// </summary>
        public List<AdditionalData> Additional_Data { get; set;}
    }

    public class AdditionalData
    {
        /// <summary>
        /// 视频描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 作者头像的 URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 作者昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 作者签名
        /// </summary>
        public string Signature { get; set; }
    }
}
