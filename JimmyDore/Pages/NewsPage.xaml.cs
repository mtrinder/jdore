using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace JimmyDore.Pages
{
    public partial class NewsPage : ContentPage
    {
        public NewsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(() => NewsList.SelectedItem = null);
        }
    }
}
