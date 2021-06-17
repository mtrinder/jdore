using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JimmyDore.Pages
{
    public partial class CurtainsPage : ContentPage
    {
        public CurtainsPage()
        {
            InitializeComponent();

            var onSource = ImageSource.FromResource("JimmyDore.Images.jd-show.png");

            var offSource = ShowLogo.Source;

            void on() => Device.BeginInvokeOnMainThread(() => ShowLogo.Source = onSource);
            void off() => Device.BeginInvokeOnMainThread(() => ShowLogo.Source = offSource);

            Device.BeginInvokeOnMainThread(() =>
            {
                Task.Delay(500).ContinueWith(t =>
                {
                    on();
                    Task.Delay(200).ContinueWith(_ => off());
                    Task.Delay(400).ContinueWith(_ => on());
                    Task.Delay(600).ContinueWith(_ => off());
                    Task.Delay(1000).ContinueWith(_ => on());
                    Task.Delay(1400).ContinueWith(_ => off());
                    Task.Delay(2000).ContinueWith(_ => on());
                });
            });
        }
    }
}
