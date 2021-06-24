using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace JimmyDore.Pages
{
    public partial class PodcastPage : ContentPage
    {
        public PodcastPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(() => AudioList.SelectedItem = null);
        }
    }
}
