using Newtonsoft.Json;

namespace M3u8Downloader_H._555dd7.plugin.Models
{
    internal class Response
    {
        public int Code { get; set; }

        public Data? Data { get; set; }

        [JsonProperty("message")]
        public string? Msg { get; set; }
    }

    internal class  Data
    {
        public Uri Url { get; set; } = default!;
    }
}
