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
using JimmyDore.Service.Podcasts;
using JimmyDore.Services.DialogAlert;
using MvvmHelpers;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;

namespace JimmyDore.ViewModels
{
    public class PodcastPageViewModel : ViewModelBase
    {
        bool _active;

        DelegateCommand _listRefresh;
        DelegateCommand _onMicButtonPress;

        public PodcastPageViewModel(
            IPodcastService podcastService,
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
            SeparatorColor = "#8e79d9";
            _podcastService = podcastService;
        }

        ObservableRangeCollection<Podcast> _podcasts = new ObservableRangeCollection<Podcast>();
        public ObservableRangeCollection<Podcast> Podcasts
        {
            get => _podcasts;
            set => SetProperty(ref _podcasts, value);
        }

        Podcast _podcastSelected;
        Podcast _podcastLastSelected;
        public Podcast PodcastSelected
        {
            get => _podcastSelected;
            set => SetProperty(ref _podcastSelected, value, OnPodcastTapped);
        }

        string _separatorColor;
        public string SeparatorColor
        {
            get => _separatorColor;
            set => SetProperty(ref _separatorColor, value);
        }

        bool _isRefreshing;
        private readonly IPodcastService _podcastService;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public DelegateCommand OnMicButtonPress => _onMicButtonPress ?? (_onMicButtonPress = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnPodcastsAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        public DelegateCommand ListRefreshCommand => _listRefresh ?? (_listRefresh = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnRefreshAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        private async Task OnPodcastsAsync()
        {
            await Xamarin.Essentials.Launcher.OpenAsync("https://jimmydorecomedy.com/the-jimmy-dore-show-podcast/");
        }

        protected override async void OnActiveChanged(bool active)
        {
            if(!_active)
            {
                _active = true;

                if (Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.None)
                {
                    await DialogService.DisplayAlertWithOk("Internet?", "Please check your connection then swipe down to refresh.");
                    return;
                }

                SeparatorColor = "AliceBlue";

                try
                {
                    var pods = await _podcastService.RssParse(App.PodcastUrl);
                    Podcasts.AddRange(pods);
                }
                finally
                {
                    SeparatorColor = "#8e79d9";
                }
            }
        }

        private async Task OnRefreshAsync()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.None)
            {
                await DialogService.DisplayAlertWithOk("Internet?", "Please check your connection then swipe down to refresh.");
                return;
            }

            SeparatorColor = "AliceBlue";

            try
            {
                var pods = await _podcastService.RssParse(App.PodcastUrl);

                Podcasts.Clear();

                Podcasts.AddRange(pods);
            }
            finally
            {
                IsRefreshing = false;
                SeparatorColor = "#8e79d9";
            }
        }

        async void OnPodcastTapped()
        {
            if (PodcastSelected != null)
            {
                if (_podcastLastSelected != null)
                {
                    _podcastLastSelected.ResetColor();
                }

                _podcastLastSelected = _podcastSelected;

                PodcastSelected.Color = "Gray";

                var navParams = new NavigationParameters
                {
                    { NavigationParameterKeys.PodcastLink, PodcastSelected.Link },
                };

                await NavigationService.NavigateAsync($"{nameof(PlayPodcastPage)}", navParams);

                PodcastSelected.ResetColor();
            }
        }
    }
}
