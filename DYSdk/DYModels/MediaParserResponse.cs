using System.Text.Json;

namespace day1.DYSdk.DYModels
{
    public class MediaParserResponse
    {
        public int Retcode { get; set; }
        public string Retdesc { get; set; }
        public bool Succ { get; set; }
        public MediaParserData Data { get; set; }
    }

    public class MediaParserData
    {
        public string Video_Id { get; set; }
        public string Platform { get; set; }
        public string Title { get; set; }
        public string Video_Url { get; set; }
        public string Audio_Url { get; set; }
        public string Cover_Url { get; set; }
        public MediaParserAuthor Author { get; set; }
        public JsonElement Image_List { get; set; }
    }

    public class MediaParserAuthor
    {
        public string Nickname { get; set; }
        public string Author_Id { get; set; }
        public string Avatar { get; set; }
    }
}
