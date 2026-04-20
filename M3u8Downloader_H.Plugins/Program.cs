using System.Text.Json;
using M3u8Downloader_H.Plugins.Models;

namespace M3u8Downloader_H.Plugins;

class Program
{
    static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    }; 
    
    static void Main(string[] args)
    {
        Console.WriteLine("RegistryBuilder start...");
        
        var payloadJson = Environment.GetEnvironmentVariable("PAYLOAD");
        if (!string.IsNullOrEmpty(payloadJson))
        {
            Console.WriteLine("Processing payload...");
            
            var payload = JsonSerializer.Deserialize<DispatchPayload>(payloadJson,JsonOptions)
                    ?? throw new NullReferenceException(nameof(DispatchPayload));
            var builder = new RegisterBuilder(payload);
            builder.ProcessPayload();
            builder.BuildRegistry();
        }
        else
        {
            Console.WriteLine("No payload, rebuild only...");
        }
        Console.WriteLine("Done.");
    }
}