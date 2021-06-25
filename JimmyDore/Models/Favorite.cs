using System;
using Prism.Mvvm;

namespace JimmyDore.Models
{
    public class Favorite : BindableBase, ICloneable
    {
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Link { get; set; }
        public string Id { get; set; }

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
            return new Favorite
            {
                Title = this.Title,
                ShortTitle = this.ShortTitle,
                Link = this.Link,
                Id = this.Id,
                Color = this.Color
            };
        }
    }
}
