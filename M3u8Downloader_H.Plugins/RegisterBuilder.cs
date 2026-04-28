using System.Net;
using System.Text;
using System.Text.Json;
using M3u8Downloader_H.Plugins.Models;

namespace M3u8Downloader_H.Plugins;

public class RegisterBuilder
{
    private readonly DispatchPayload _payload;
    private readonly string _dataDirectory = "data";
    private readonly string _pluginDirectory;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public RegisterBuilder(DispatchPayload payload)
    {
        _payload = payload;
        _pluginDirectory  = Path.Combine(_dataDirectory,"plugins");
        Directory.CreateDirectory(_pluginDirectory);
    }

    public void ProcessPayload()
    {
        var file = Path.Combine(_pluginDirectory, $"{_payload.Key}.json");

        PluginVersionFile data;
        
        if (File.Exists(file))
        {
            data = JsonSerializer.Deserialize<PluginVersionFile>(File.ReadAllText(file),_jsonOptions) 
                   ?? throw new InvalidOperationException(nameof(PluginVersionFile));
            
            data.Versions.RemoveAll(r => r.Release.Version == _payload.Release.Version);
        }
        else
        {
            data = new PluginVersionFile
            {
                Key = _payload.Key,
            };
        }
        
        data.Versions.Add(
            new PluginVersion()
            {
                Manifest = _payload.Manifest,
                Release = _payload.Release,
                Runtime =  _payload.Runtime
            });
            
        data.Versions = [.. data.Versions.OrderByDescending(r => r.Release.Version)];
        
        File.WriteAllText(file, JsonSerializer.Serialize(data, _jsonOptions));
    }

    public void BuildRegistry()
    {
        var file = Path.Combine(_dataDirectory,"plugins.json");
        
        PluginRegistry data;
        if (File.Exists(file))
        {
            data =  JsonSerializer.Deserialize<PluginRegistry>(File.ReadAllText(file), _jsonOptions)
                    ?? throw new InvalidOperationException(nameof(PluginRegistry));

            var payload = data.DispatchPayloads.FirstOrDefault(d => d.Key == _payload.Key);
            if(payload != null)
                data.DispatchPayloads.Remove(payload);
        }
        else
        {
            data = new PluginRegistry();
        }
        
        data.DispatchPayloads.Add(_payload);

        data.DispatchPayloads = data.DispatchPayloads.OrderByDescending(d => d.Manifest.Time).ToList();
        File.WriteAllText(file, JsonSerializer.Serialize(data, _jsonOptions));

        BuildReadme(data);
    }
    
    private void BuildReadme(PluginRegistry  data)
    {
        var file = Path.Combine(_dataDirectory,"readmeHead.md");
        var headContent = File.ReadAllText(file);

        using var readmeFile = File.Create("README.md");
        using var writer = new StreamWriter(readmeFile);
        writer.Write(headContent);
        writer.Flush();
        
        writer.WriteLine(writer.NewLine);
        writer.WriteLine("|名字|描述|当前版本|更新时间|");
        writer.WriteLine("|:--:|:--:|:--:|:--:|");
        foreach (var item in data.DispatchPayloads)
        {
            writer.Write('|');
            writer.Write($"[{item.Manifest.Title}]({item.Manifest.Repo})");
            writer.Write('|');
            writer.Write(item.Manifest.Description);
            writer.Write('|');
            writer.Write(item.Release.Version);
            writer.Write('|');
            writer.Write(item.Manifest.Time);
            writer.WriteLine("|");
        }
    }
}