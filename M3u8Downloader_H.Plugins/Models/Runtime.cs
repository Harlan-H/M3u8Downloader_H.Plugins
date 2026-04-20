namespace M3u8Downloader_H.Plugins.Models;

public class Runtime
{
    public Capabilities Capabilities { get; set; } = default!;
    public string EntryPoint { get; set; } = default!;
    public string WorkingDir { get; set; } = default!;

}