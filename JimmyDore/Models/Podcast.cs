using System;
using Prism.Mvvm;

namespace JimmyDore.Models
{
    public class Podcast : BindableBase, ICloneable
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

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
            return new Podcast
            {
                Title = this.Title,
                Link = this.Link,
                Date = this.Date,
                Description = this.Description
            };
        }
    }
}
