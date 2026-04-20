namespace M3u8Downloader_H.Plugins.Models;

public class PluginVersionFile
{
    public string Key { get; set; }  = default!;
    public List<PluginVersion> Versions { get; set; } = [];
}