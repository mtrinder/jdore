using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using FeedParserCore;
using JimmyDore.Extensions;
using JimmyDore.Models;
using JimmyDore.Pages;
using JimmyDore.Services.DialogAlert;
using MvvmHelpers;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;

namespace JimmyDore.ViewModels
{
    public class FavoritesPageViewModel : ViewModelBase
    {
        bool _active;

        public FavoritesPageViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
            SeparatorColor = "DarkBlue";
        }

        ObservableRangeCollection<Favorite> _channels = new ObservableRangeCollection<Favorite>();
        public ObservableRangeCollection<Favorite> Channels
        {
            get => _channels;
            set => SetProperty(ref _channels, value);
        }

        Favorite _channelSelected;
        Favorite _channelLastSelected;
        public Favorite ChannelSelected
        {
            get => _channelSelected;
            set => SetProperty(ref _channelSelected, value, OnChannelTapped);
        }

        string _separatorColor;
        public string SeparatorColor
        {
            get => _separatorColor;
            set => SetProperty(ref _separatorColor, value);
        }

        protected override async void OnActiveChanged(bool active)
        {
            if (!_active)
            {
                _active = true;

                SeparatorColor = "AliceBlue";

                try
                {
                    var channels = new ObservableRangeCollection<Favorite>();

                    channels.Add(new Favorite
                    {
                        Title = "Ron Placone",
                        ShortTitle = "Ron Placone",
                        Id = "PLFFtqmK9LG3q3ii6OsSkst2VOZNgfgI2x", //"UCwFnxeT6z_ZO -fG2zGubPLw",
                        Link = "https://pbs.twimg.com/profile_images/1318794658403606528/z9zjn9gK.jpg"
                    });

                    channels.Add(new Favorite
                    {
                        Title = "Bad Faith",
                        ShortTitle = "Bad Faith",
                        Id = "UULNw2JNuTUWFdilpzOXLa5A",
                        Link = "https://pbs.twimg.com/profile_images/1332427701567823881/ObL-RFX7.jpg"
                    });

                    channels.Add(new Favorite
                    {
                        Title = "The Grayzone",
                        ShortTitle = "The Grayzone",
                        Id = "UUEXR8pRTkE2vFeJePNe9UcQ",
                        Link = "https://pbs.twimg.com/profile_images/1309982860582019076/NOfYOKvY_400x400.jpg"
                    });

                    channels.Add(new Favorite
                    {
                        Title = "The Takeover with Justin Jackson",
                        ShortTitle = "Justin Jackson",
                        Id = "UUjPc-CPhp3DsivpkkmBUgew",
                        Link = "https://pbs.twimg.com/profile_images/1346280537883701248/t9ilONYc_400x400.jpg"
                    });

                    channels.Add(new Favorite
                    {
                        Title = "The Katie Halper Show",
                        ShortTitle = "Katie Halper",
                        Id = "UUUsMkDtVyel9USjCTaM42rw",
                        Link = "https://pbs.twimg.com/profile_images/790656974375583744/kCF3pSDg_400x400.jpg"
                    });

                    channels.Add(new Favorite
                    {
                        Title = "Graham Elwood",
                        ShortTitle = "Graham Elwood",
                        Id = "UUX1rle36wIlP9ry8uidnCsA",
                        Link = "https://pbs.twimg.com/profile_images/839669140163973120/9UfoUytx.jpg"
                    });

                    Channels.AddRange(channels);
                }
                finally
                {
                    SeparatorColor = "DarkBlue";
                }
            }
        }

        async void OnChannelTapped()
        {
            if (ChannelSelected != null)
            {
                if (_channelLastSelected != null)
                {
                    _channelLastSelected.ResetColor();
                }

                _channelLastSelected = _channelSelected;

                ChannelSelected.Color = "Gray";

                var navParams = new NavigationParameters
                {
                    { NavigationParameterKeys.Title, ChannelSelected.ShortTitle },
                    { NavigationParameterKeys.ChannelId, ChannelSelected.Id },
                };

                if (Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.None)
                {
                    await DialogService.DisplayAlertWithOk("Internet?", "Please check your connection then try again.");
                    return;
                }

                await NavigationService.NavigateAsync($"{nameof(ChannelPage)}", navParams);

                ChannelSelected.ResetColor();
            }
        }
    }
}
