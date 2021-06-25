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
        const int count = 25;
        const string uri = "https://youtube.googleapis.com";
        string key = "8UTfMLgMdJqFcU4J5GqU7Dd2YcmW3TpHAySazIA";

        ObservableRangeCollection<Video> _jimmysVideos;

        public YouTubeService()
        {
            key = new string(key.ToCharArray().Reverse().ToArray());
        }

        public async Task<ObservableRangeCollection<Video>> GetJimmysVideos(bool refresh)
        {
            if (refresh || _jimmysVideos == null)
            {
                var getstats = false;

                try
                {
                    var jimmysVideos = await GetPlaylistForChannel("UU3M7l8ved_rYQ45AVzS0RGA");

                    if (_jimmysVideos != null)
                    {
                        _jimmysVideos.Clear();
                    }

                    _jimmysVideos = jimmysVideos;

                    getstats = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    MessagingCenter.Send<IYouTubeService>(this, "Video-Retrieve-Failed");
                }

                try
                {
                    if (getstats)
                    {
                        await GetStats(_jimmysVideos);

                        MessagingCenter.Send<IYouTubeService>(this, "Video-Stats-Retrieved");
                    }
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

        public async Task<ObservableRangeCollection<Video>> GetPlaylistForChannel(string channel)
        {
            ObservableRangeCollection<Video> videos = new ObservableRangeCollection<Video>();

            using (HttpClient _httpClient = new HttpClient())
            {
                var result = await _httpClient.GetStringAsync($"{uri}/youtube/v3/playlistItems?part=snippet&maxResults={count}&playlistId={channel}&key={key}");

                var videosResult = JsonConvert.DeserializeObject<YouTubeResult>(result);

                foreach (var item in videosResult.Items)
                {
                    var video = new Video
                    {
                        Title = item.Snippet.Title + "                    ",
                        Link = item.Snippet.Thumbnails.Medium.Url,
                        VideoId = item.Snippet.ResourceId.VideoId,
                        Date = item.Snippet.PublishedAt.ToString("ddd, MMMM dd, yyyy htt"),
                        Funny = item.Snippet.Description.Contains("Performed by Mike MacRae")
                    };

                    videos.Add(video);
                }
            }

            return videos;
        }

        private async Task GetStats(ObservableRangeCollection<Video> videos)
        {
            var ids = videos.Select(x => x.VideoId).ToList();

            using (HttpClient _httpClient = new HttpClient())
            {
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

        private async Task TestGet()
        {
            try
            {
                using (HttpClient _httpClient = new HttpClient())
                {
                    var result = await _httpClient.GetAsync("https://www.google.com");

                    result.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
