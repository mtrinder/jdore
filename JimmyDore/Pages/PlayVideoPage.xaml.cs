using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace JimmyDore.Pages
{
    public partial class PlayVideoPage : ContentPage
    {
        public PlayVideoPage()
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
