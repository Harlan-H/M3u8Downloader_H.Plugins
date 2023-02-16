using M3u8Downloader_H._555dd7.plugin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H._555dd7.plugin.Models
{
    internal readonly struct PlayerRequest
    {
        private readonly string playerUrl;
        private readonly string _userAgent;
        public PlayerRequest(string playerUrl, string userAgent)
        {
            this.playerUrl = playerUrl + "/get_play_url";
            _userAgent = userAgent;
        }

        public string AppKey =>  "www.555dy.com".GetMD5HexString().ToLower();
        public string ClientKey => _userAgent.GetMD5HexString().ToLower();
        public string RequestToken => "https://zyz.sdljwomen.com".GetMD5HexString().ToLower();
        public string AccessToken => playerUrl.Replace("https:","").GetMD5HexString().ToLower();

        public string PlayUrl => playerUrl;

        public static implicit operator Uri(PlayerRequest playerRequest) =>
            new($"{playerRequest.PlayUrl}?app_key={playerRequest.AppKey}&client_key={playerRequest.ClientKey}&request_token={playerRequest.RequestToken}&access_token={playerRequest.AccessToken}");
    }
}
