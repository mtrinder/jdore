using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace JimmyDore.Pages
{
    public partial class PlayPodcastPage : ContentPage
    {
        public PlayPodcastPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (webView != null)
            {
                webView.Source = new UrlWebViewSource { Url = "about:blank" };
            }
        }
    }
}
