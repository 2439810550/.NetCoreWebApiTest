namespace day1.DTOs
{
    public class DYIRemoveVideoWatermarkDTO
    {
            public string VideoUrl { get; set; }      // 无水印视频地址
            public string PlayUrl { get; set; }       // 播放地址
            public string Description { get; set; }   // 视频描述
            public string AuthorNickname { get; set; } // 作者昵称
            public string AuthorAvatar { get; set; }   // 作者头像
    }
}
