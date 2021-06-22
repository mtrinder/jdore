using System;
using System.Collections.Generic;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;

namespace JimmyDore.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(() => VideoList.SelectedItem = null);
        }
    }
}
