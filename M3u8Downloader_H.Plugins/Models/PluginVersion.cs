namespace M3u8Downloader_H.Plugins.Models;

public class PluginVersion
{
    public Manifest Manifest { get; set; } = default!;
    public Release Release { get; set; } = default!;
    public Runtime Runtime { get; set; } = default!;
}