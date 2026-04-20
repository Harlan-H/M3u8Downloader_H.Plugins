using System.Text.Json.Serialization;

namespace M3u8Downloader_H.Plugins.Models;

public class PluginRegistry
{
    [JsonPropertyName("plugins")]
    public List<DispatchPayload> DispatchPayloads { get; set; } = [];
}