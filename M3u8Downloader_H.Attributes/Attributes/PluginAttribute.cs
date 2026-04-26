using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace M3u8Downloader_H.Attributes.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute(string title, string description, string author, string appVersion) : Attribute
    {
        public string Key { get; set; } = string.Empty;
        public string Title { get; } = title;
        public string Description { get; } = description;
        public string Author { get; } = author;
        public string AppVersion { get; } = appVersion;

        public bool HasUi { get; set; } = false;
        public bool HasDownload { get; set; } = false;
    }
}
