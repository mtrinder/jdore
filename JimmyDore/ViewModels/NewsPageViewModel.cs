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
    public class NewsPageViewModel : ViewModelBase
    {
        bool _active;

        public NewsPageViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
            SeparatorColor = "#bf6011";
        }

        ObservableRangeCollection<NewsAlert> _newsItems = new ObservableRangeCollection<NewsAlert>();
        public ObservableRangeCollection<NewsAlert> NewItems
        {
            get => _newsItems;
            set => SetProperty(ref _newsItems, value);
        }

        NewsAlert _alertSelected;
        NewsAlert _alertLastSelected;
        public NewsAlert AlertSelected
        {
            get => _alertSelected;
            set => SetProperty(ref _alertSelected, value, OnChannelTapped);
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
                    var news = new ObservableRangeCollection<NewsAlert>();

                    news.Add(new NewsAlert
                    {
                        Title = "U.S. Case Against Julian Assange Falls Apart, As Key Witness Says He Lied",
                        Link = "https://www.realclearpolitics.com/video/2021/07/04/attorney_us_case_against_julian_assange_falls_apart_as_key_witness_says_he_lied.html",
                        Date = DateTime.Now.ToString("ddd, MMMM dd, yyyy htt")
                    });
                    NewItems.AddRange(news);
                }
                finally
                {
                    SeparatorColor = "#dc1e1e";
                }
            }
        }

        async void OnChannelTapped()
        {
            if (AlertSelected != null)
            {
                if (_alertLastSelected != null)
                {
                    _alertLastSelected.ResetColor();
                }

                _alertLastSelected = _alertSelected;

                AlertSelected.Color = "Gray";

                //var navParams = new NavigationParameters
                //{
                //    { NavigationParameterKeys.Title, ChannelSelected.ShortTitle },
                //    { NavigationParameterKeys.ChannelId, ChannelSelected.Id },
                //};

                //if (Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.None)
                //{
                //    await DialogService.DisplayAlertWithOk("Internet?", "Please check your connection then try again.");
                //    return;
                //}

                //await NavigationService.NavigateAsync($"{nameof(ChannelPage)}", navParams);

                AlertSelected.ResetColor();
            }
        }
    }
}
