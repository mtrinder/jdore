using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JimmyDore.Models;
using MvvmHelpers;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace JimmyDore.Service.YouTube
{
    public class YouTubeService : IYouTubeService
    {
        const int count = 50;
        const string uri = "https://youtube.googleapis.com";
        private readonly HttpClient _httpClient;

        string key;

        string key1 = "4mw3e_NffaO5ScQzq357gbC3y431C7UrDySazIA";
        string key2 = "oAsCvemIE9RX6JIkiucKSXOgupJ0FgmjAySazIA";
        string key3 = "UzGl18JS4SdEsXONtwNoKn1h3zqUCU3hCySazIA";
        string key4 = "oST_8PJ7zRmSkPT2qXo8E9a0ODM2y9B-AySazIA";
        string key5 = "gHse2v2llaR0tkWkjqEn811eceoHhbs1AySazIA";
        string key6 = "gwrNfem-tswtX_TeeDMbL_TZkgIoBKJICySazIA";
        string key7 = "E5Ri7oKOfUvMTSrKxAcgqsEdFRRIPBZqCySazIA";

        string jimmysChannel = "UU3M7l8ved_rYQ45AVzS0RGA";

        ObservableRangeCollection<Video> _jimmysVideos;

        public YouTubeService(HttpClient httpClient)
        {
            var offset = TimeZoneInfo.Local.BaseUtcOffset;
            var hours = offset.TotalHours;

            key = key7;

            if (hours <= -3 && hours >= -8)
            {
                if (hours <= -3) key = key1;
                if (hours <= -4) key = key2;
                if (hours <= -5) key = key3;
                if (hours <= -6) key = key4;
                if (hours <= -7) key = key5;
                if (hours <= -8) key = key6;
            }

            key = new string(key.ToCharArray().Reverse().ToArray());

            _httpClient = httpClient;
        }

        public async Task<ObservableRangeCollection<Video>> GetJimmysVideos(bool refresh)
        {
            if (refresh || _jimmysVideos == null)
            {
                try
                {
                    var jimmysVideos = await GetPlaylistForChannel(jimmysChannel, count);

                    if (_jimmysVideos != null)
                    {
                        _jimmysVideos.Clear();
                    }

                    _jimmysVideos = jimmysVideos;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    MessagingCenter.Send<IYouTubeService>(this, "Video-Retrieve-Failed");
                }

                return _jimmysVideos;
            }

            return _jimmysVideos;
        }

        public async Task<ObservableRangeCollection<Video>> GetPlaylistForChannel(string channel, int maxCount)
        {
            ObservableRangeCollection<Video> videos = new ObservableRangeCollection<Video>();

            var result = await _httpClient.GetStringAsync($"{uri}/youtube/v3/playlistItems?part=snippet&maxResults={maxCount}&playlistId={channel}&key={key}");

            var videosResult = JsonConvert.DeserializeObject<YouTubeResult>(result);

            foreach (var item in videosResult.Items)
            {
                if (item.Snippet.Title.Length > 80)
                {
                    item.Snippet.Title = item.Snippet.Title.Substring(0, 80) + "...";
                }

                var video = new Video(this, item.Snippet.ResourceId.VideoId)
                {
                    Title = item.Snippet.Title + "                    ",
                    Link = item.Snippet.Thumbnails.Default.Url,
                    Date = item.Snippet.PublishedAt.ToString("ddd, MMMM dd, yyyy htt"),
                    Funny = item.Snippet.Description.Contains("Performed by Mike MacRae")
                };

                videos.Add(video);
            }

            if (channel != jimmysChannel)
            {
                MessagingCenter.Send<IYouTubeService>(this, "Video-Stats-Retrieved");
            }

            return videos;
        }

        public async Task<Stats> GetStatisticsForVideo(string videoId)
        {
            var likes = "";
            var views = "";
            try
            {
                var result = await _httpClient.GetStringAsync($"{uri}/youtube/v3/videos?part=statistics&id={videoId}&key={key}");

                var stats = JsonConvert.DeserializeObject<VideoResult>(result);

                likes = (int.Parse(stats.Items[0].Statistics.LikeCount) / 1000).ToString("0.#k") + " Likes";
                views = (int.Parse(stats.Items[0].Statistics.ViewCount) / 1000).ToString("0.#k") + " Views";
            }
            catch (Exception)
            {

            }

            return new Stats { Likes = likes, Views = views };
        }

        private async Task GetStats(ObservableRangeCollection<Video> videos)
        {
            var ids = videos.Select(x => x.VideoId).ToList();

            foreach (var id in ids)
            {
                var result = await _httpClient.GetStringAsync($"{uri}/youtube/v3/videos?part=statistics&id={id}&key={key}");

                var stats = JsonConvert.DeserializeObject<VideoResult>(result);

                var video = videos.FirstOrDefault(x => x.VideoId.Equals(id));
                var index = videos.IndexOf(video);

                video.Likes = (int.Parse(stats.Items[0].Statistics.LikeCount) / 1000).ToString("0.#k") + " Likes";
                video.Views = (int.Parse(stats.Items[0].Statistics.ViewCount) / 1000).ToString("0.#k") + " Views";
                videos[index] = video.Clone() as Video;
            }
        }
    }
}
