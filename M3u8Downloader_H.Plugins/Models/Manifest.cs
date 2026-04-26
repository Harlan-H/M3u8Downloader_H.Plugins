namespace M3u8Downloader_H.Plugins.Models;

public class Manifest
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string Repo { get; set; } = default!;
    public DateTime Time { get; set; } = DateTime.UtcNow + new TimeSpan(8, 0, 0);
}