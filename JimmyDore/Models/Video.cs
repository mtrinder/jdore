using System;
using JimmyDore.Service.YouTube;
using Prism.Mvvm;
using Xamarin.Forms;

namespace JimmyDore.Models
{
    public class Stats
    {
        public string Likes { get; set; }
        public string Views { get; set; }
    }

    public class Video : BindableBase, ICloneable
    {
        public Video(IYouTubeService youTubeService, string videoId)
        {
            VideoId = videoId;

            //if (youTubeService != null)
            //{
            //    Device.BeginInvokeOnMainThread(async () =>
            //    {
            //        var stats = await youTubeService.GetStatisticsForVideo(videoId);

            //        try
            //        {
            //            Views = stats.Views;
            //            Likes = stats.Likes;
            //        }
            //        catch (Exception)
            //        {

            //        }
            //    });
            //}

            ResetColor();
        }

        public string Title { get; set; }
        public string Link { get; set; }
        public string VideoId { get; set; }
        public string Date { get; set; }
        public bool Funny { get; set; }

        string _likes;
        public string Likes
        {
            get => _likes;
            set => SetProperty(ref _likes, value);
        }

        string _views;
        public string Views
        {
            get => _views;
            set => SetProperty(ref _views, value);
        }

        string _color;
        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public void ResetColor()
        {
            Color = "Black";
        }

        public object Clone()
        {
            return new Video(null, this.VideoId)
            {
                Title = this.Title,
                Link = this.Link,
                Likes = this.Likes,
                Views = this.Views,
                Date = this.Date,
                Funny = this.Funny,
                Color = this.Color
            };
        }
    }
}
