using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace JimmyDore.Pages
{
    public partial class FavoritesPage : ContentPage
    {
        public FavoritesPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(() => ChannelsList.SelectedItem = null);
        }
    }
}
