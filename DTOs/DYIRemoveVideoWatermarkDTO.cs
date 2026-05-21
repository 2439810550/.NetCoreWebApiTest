namespace day1.DTOs
{
    /// <summary>
    /// 抖音去水印解析结果
    /// </summary>
    public class DYIRemoveVideoWatermarkDTO
    {
        /// <summary>video / image / mixed</summary>
        public string MediaType { get; set; }

        /// <summary>无水印视频地址（video/mixed 类型有值）</summary>
        public string VideoUrl { get; set; }

        /// <summary>播放地址（同 VideoUrl）</summary>
        public string PlayUrl { get; set; }

        /// <summary>视频描述/标题</summary>
        public string Description { get; set; }

        /// <summary>作者昵称</summary>
        public string AuthorNickname { get; set; }

        /// <summary>作者头像</summary>
        public string AuthorAvatar { get; set; }

        /// <summary>封面图</summary>
        public string CoverUrl { get; set; }

        /// <summary>背景音乐地址</summary>
        public string AudioUrl { get; set; }

        /// <summary>图集列表（image/mixed 类型有值）</summary>
        public List<MediaImageItem> Images { get; set; }
    }

    public class MediaImageItem
    {
        /// <summary>图片地址</summary>
        public string Url { get; set; }

        /// <summary>Live Photo/动图地址（可能为空）</summary>
        public string LivePhotoUrl { get; set; }
    }
}
