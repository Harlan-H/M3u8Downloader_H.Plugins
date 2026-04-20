namespace M3u8Downloader_H.Plugins.Models;

public class Release
{
    public Version Version { get; set; } = default!;
    public Uri DownloadUrl { get; set; } = default!;
    public Version MinAppVersion { get; set; } = default!;
}