using System;
using Prism.Mvvm;

namespace JimmyDore.Models
{
    public class Video : BindableBase, ICloneable
    {
        public Video()
        {
            ResetColor();
        }

        public string Title { get; set; }
        public string Link { get; set; }
        public string VideoId { get; set; }
        public string Likes { get; set; }
        public string Views { get; set; }
        public string Date { get; set; }
        public bool Funny { get; set; }

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
            return new Video
            {
                Title = this.Title,
                Link = this.Link,
                VideoId = this.VideoId,
                Likes = this.Likes,
                Views = this.Views,
                Date = this.Date,
                Funny = this.Funny,
                Color = this.Color
            };
        }
    }
}
