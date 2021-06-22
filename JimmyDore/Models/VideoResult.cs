using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace JimmyDore.Models
{
    public class VideoResult
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("items")]
        public List<VideoItem> Items { get; set; }

        [JsonProperty("pageInfo")]
        public VideoPageInfo PageInfo { get; set; }
    }

    public class Statistics
    {
        [JsonProperty("viewCount")]
        public string ViewCount { get; set; }

        [JsonProperty("likeCount")]
        public string LikeCount { get; set; }

        [JsonProperty("dislikeCount")]
        public string DislikeCount { get; set; }

        [JsonProperty("favoriteCount")]
        public string FavoriteCount { get; set; }

        [JsonProperty("commentCount")]
        public string CommentCount { get; set; }
    }

    public class VideoItem
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("statistics")]
        public Statistics Statistics { get; set; }
    }

    public class VideoPageInfo
    {
        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("resultsPerPage")]
        public int ResultsPerPage { get; set; }
    }
}

