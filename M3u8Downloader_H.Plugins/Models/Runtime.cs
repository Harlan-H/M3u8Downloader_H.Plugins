namespace M3u8Downloader_H.Plugins.Models;

public class Runtime
{
    public string EntryPoint { get; set; } = default!;
    public string EntryType { get; set; } = default!;
    public bool HasUi { get; set; } = default!;
    public bool HasDownload { get; set; } = default!;

}