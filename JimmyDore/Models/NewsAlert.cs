using System;
using Prism.Mvvm;

namespace JimmyDore.Models
{
    public class NewsAlert : BindableBase, ICloneable
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Date { get; set; }

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
            return new NewsAlert()
            {
                Title = this.Title,
                Link = this.Link,
                Date = this.Date,
                Color = this.Color
            };
        }
    }
}
