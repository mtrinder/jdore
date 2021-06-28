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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(500);
                    ShowLogo.IsVisible = true;
                    await Task.Delay(200);
                    ShowLogo.IsVisible = false;
                    await Task.Delay(400);
                    ShowLogo.IsVisible = true;
                });
                return false;
            });
        }
    }
}
