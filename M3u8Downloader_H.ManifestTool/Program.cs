using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace M3u8Downloader_H.ManifestTool
{
    internal class Program
    {
        private static readonly JsonSerializerOptions opts = new()
        {
            WriteIndented = true
        };
        static void Main(string[] args)
        {
            var dllPath = Path.GetFullPath(args[0]);
            var baseDir = Path.GetDirectoryName(dllPath)!;

            var downloadUrl = args.Length > 1 ? args[1] : "";
            var repo = args.Length > 2 ? args[2] : "";

            var runtimeAssemblies = Directory.GetFiles(baseDir, "*.dll");

            // .NET runtime 也要加，否则基础类型解析不了
            var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            var coreAssemblies = Directory.GetFiles(coreDir, "*.dll");

            // 为了加载Attributes.dll
            var attributeDir = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
            var attriAssemblies = Directory.GetFiles(attributeDir, "*.dll");

            var resolver = new PathAssemblyResolver(runtimeAssemblies.Concat(coreAssemblies).Concat(attriAssemblies));
            using var mlc = new MetadataLoadContext(resolver);

            var assembly = mlc.LoadFromAssemblyPath(dllPath);

            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => t.GetCustomAttributesData()
                    .Any(a => a.AttributeType.Name == "PluginAttribute")) 
                ?? throw new Exception("找不到带 [Plugin] 特性的类型");

            var attrData = pluginType.GetCustomAttributesData()
                .First(a => a.AttributeType.Name == "PluginAttribute");

            var ctorArgs = attrData.ConstructorArguments;
            var namedArgs = attrData.NamedArguments;

            var title = ctorArgs[0].Value?.ToString() ?? "";
            var description = ctorArgs[1].Value?.ToString() ?? "";
            var author = ctorArgs[2].Value?.ToString() ?? "";
            var appVersion = ctorArgs[3].Value?.ToString() ?? "";

            var dllKey = attrData.NamedArguments.FirstOrDefault(n => n.MemberName == "Key").TypedValue.Value?.ToString();
            var hasui = attrData.NamedArguments.FirstOrDefault(n => n.MemberName == "HasUi").TypedValue.Value?.ToString();
            var hasdownload = attrData.NamedArguments.FirstOrDefault(n => n.MemberName == "HasDownload").TypedValue.Value?.ToString();

            var key = string.IsNullOrEmpty(dllKey) ? assembly.GetName().Name!.ToLower(): dllKey;

            var version = FileVersionInfo
                .GetVersionInfo(dllPath)
                .ProductVersion ?? "1.0.0";


            bool hasUI = !string.IsNullOrEmpty(hasui) && Convert.ToBoolean(hasui);
            bool hasDownload = !string.IsNullOrEmpty(hasdownload) && Convert.ToBoolean(hasdownload);


            var entryType = pluginType.FullName!;


            var manifest = new 
            {
                Key = key,
                Manifest = new 
                {
                    Title = title,
                    Description = description,
                    Author = author,
                    Repo = repo,
                },
                Release = new 
                {
                    Version = version,
                    DownloadUrl = downloadUrl,
                    MinAppVersion = appVersion,
                },
                Runtime = new 
                {
                    EntryPoint = Path.GetFileName(dllPath),
                    EntryType = entryType,
                    HasUi = hasUI,
                    HasDownload = hasDownload,
                }
            };

            var json = JsonSerializer.Serialize(manifest, opts);

            File.WriteAllText("manifest.json", json);

            Console.WriteLine("manifest.json generated!");
        }
    }
}
