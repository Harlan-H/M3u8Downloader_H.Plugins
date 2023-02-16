using M3u8Downloader_H._555dd7.plugin.Models;
using M3u8Downloader_H._555dd7.plugin.Utils;
using M3u8Downloader_H.Common.Extensions;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace M3u8Downloader_H._555dd7.plugin.Streams
{
    internal class Extractor
    {
        private readonly HttpClient httpClient;
        private readonly IEnumerable<KeyValuePair<string, string>>? headers;
        private static readonly Regex regex = new("player_aaaa=(.*?)</script>", RegexOptions.Compiled);
        private static readonly Regex serverUrlRegex = new("'(https://.*?)'", RegexOptions.Compiled);
        private static readonly byte[] _key = Encoding.UTF8.GetBytes("55cc5c42a943afdc");
        private static readonly byte[] _iv = Encoding.UTF8.GetBytes("d11324dcscfe16c0");
        private static readonly string _hmacRawKey = "55ca5c4d11424dcecfe16c08a943afdc";

        public Extractor(HttpClient httpClient, IEnumerable<KeyValuePair<string, string>>? headers)
        {
            this.httpClient = httpClient;
            this.headers = headers;
        }

        public async Task<Uri> GetM3u8IndexUrl(VideoId videoId,CancellationToken cancellationToken)
        {
            Uri uri = videoId;
            var url = await GetMainPageUrl(uri, cancellationToken);
            var serverUrl = await GetServerUrl(uri,cancellationToken);
            return await GetPlayUrlFromServerUrl(url,serverUrl,cancellationToken);
        }

        private async Task<Uri> GetPlayUrlFromServerUrl(string mainpageUrl, string serverUrl, CancellationToken cancellationToken)
        {
            var tmpHeaders = GetPlayerUrlHeaders(mainpageUrl, serverUrl);
            tmpHeaders.TryGetValue("User-Agent", out string? useragent);
            Uri RequestUri = new PlayerRequest(serverUrl, useragent ?? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");
            var encryptedResp = await httpClient.GetStringAsync(RequestUri, tmpHeaders, cancellationToken);
            var decryptText = encryptedResp.ToHexBytes().AesDecrypt(_key, _iv).GetString();
            var r = JsonConvert.DeserializeObject<Response>(decryptText);
            if (r is null)
                throw new InvalidDataException("返回内容序列化失败");

            if (r.Code != 200)
                throw new InvalidDataException($"数据返回异常,错误内容:{r.Msg}");

            return r.Data!.Url;
        }

        private Dictionary<string, string> GetPlayerUrlHeaders(string url,string serverurl)
        {
            Dictionary<string, string> tmpHeaders = headers is not null ? new Dictionary<string, string>(headers) : new();
            long now = DateTimeOffset.Now.ToLocalTime().ToUnixTimeSeconds();;
            string SecondString = Convert.ToString(now);
            tmpHeaders.Add("X-PLAYER-TIMESTAMP", SecondString);
            tmpHeaders.Add("X-PLAYER-METHOD", "GET");

            var encryptedUrl = url.ToBytes().AesEncrypt(_key, _iv).GetHexString();
            tmpHeaders.Add("X-PLAYER-PACK", encryptedUrl);

            var hmacKeyBytes = (serverurl + "GET" + SecondString + _hmacRawKey).GetMD5HexString().ToLower().ToBytes();
            byte[] encryptData  = encryptedUrl.ToBytes().HmacSha256(hmacKeyBytes);
            tmpHeaders.Add("X-PLAYER-SIGNATURE", Convert.ToHexString(encryptData).ToLower());
            return tmpHeaders;
        }


        private async Task<string> GetMainPageUrl(Uri uri, CancellationToken cancellationToken)
        {
            var raw = await httpClient.GetStringAsync(uri, headers, cancellationToken);
            var initState = ExtractInitState(raw);
            PlayerInfo? playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(initState);
            if (playerInfo is null)
                throw new InvalidDataException("主页关键数据反序列化失败");

            return playerInfo.Url;
        }

        private async Task<string> GetServerUrl(Uri baseUri, CancellationToken cancellationToken = default)
        {
            Uri uri = new(baseUri, "/player.html");
            var raw = await httpClient.GetStringAsync(uri, headers, cancellationToken);
            var urlGroups = serverUrlRegex.Matches(raw);
            if (urlGroups.Count < 2)
                throw new InvalidDataException("server url地址获取失败");

            var randomNum = new Random().Next(urlGroups.Count);
            return urlGroups[randomNum].Groups[1].Value;
        }

        private static string ExtractInitState(string raw)
        {
            var initState = regex.Match(raw).Groups[1].Value;
            if (string.IsNullOrWhiteSpace(initState))
                throw new InvalidDataException("没有获取到网页关键数据");

            return initState;
        }
    }
}
