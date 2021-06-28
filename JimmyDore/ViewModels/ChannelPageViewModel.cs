using System;
using System.Threading.Tasks;
using JimmyDore.Extensions;
using JimmyDore.Models;
using JimmyDore.Pages;
using JimmyDore.Service.YouTube;
using JimmyDore.Services.DialogAlert;
using MvvmHelpers;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Xamarin.Forms;

namespace JimmyDore.ViewModels
{
    public class ChannelPageViewModel : ViewModelBase
    {
        bool _firstTime;
        private readonly IYouTubeService _youTubeService;
        private DelegateCommand _navigateOutCommand;

        public ChannelPageViewModel(
            INavigationService navigationService,
            IYouTubeService youTubeService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
            _youTubeService = youTubeService;

            ActionOutVisible = false;
            SeparatorColor = "DarkBlue";

            MessagingCenter.Subscribe<IYouTubeService>(this, "Video-Stats-Retrieved", s =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SeparatorColor = "DarkBlue";
                });
            });
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.IsNewNavigation())
            {
                PageTitle = parameters.Get<string>(NavigationParameterKeys.Title);
                ChannelId = parameters.Get<string>(NavigationParameterKeys.ChannelId);
                ChannelUrl = $"https://www.youtube.com/channel/{ChannelId}";
                
                if (Device.RuntimePlatform == Device.Android)
                {
                    Device.BeginInvokeOnMainThread(() => ShowList = true);
                }

                await OnRefreshAsync();

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        Device.BeginInvokeOnMainThread(() => ShowList = true);
                    }

                    return false;
                });
            }
        }

        public string ChannelId { get; set; }
        public string ChannelUrl { get; set; }

        bool _actionOutVisible;
        public override bool ActionOutVisible
        {
            get => _actionOutVisible;
            set => SetProperty(ref _actionOutVisible, value);
        }

        string _separatorColor;
        public string SeparatorColor
        {
            get => _separatorColor;
            set => SetProperty(ref _separatorColor, value);
        }

        bool _showList;
        public bool ShowList
        {
            get => _showList;
            set => SetProperty(ref _showList, value);
        }

        ObservableRangeCollection<Video> _videos = new ObservableRangeCollection<Video>();
        public ObservableRangeCollection<Video> Videos
        {
            get => _videos;
            set => SetProperty(ref _videos, value);
        }

        Video _videoSelected;
        Video _videoLastSelected;
        public Video VideoSelected
        {
            get => _videoSelected;
            set => SetProperty(ref _videoSelected, value, OnVideoTapped);
        }

        public DelegateCommand NavigateOutCommand => _navigateOutCommand ?? (_navigateOutCommand = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnNavigateOutCommandAsync), CanNavigateOut));

        private bool CanNavigateOut()
        {
            return Xamarin.Essentials.Launcher.CanOpenAsync(ChannelUrl).Result;
        }

        private async Task OnNavigateOutCommandAsync()
        {
            await OnNavigateOutCommandInternalAsync();
        }

        protected virtual async Task OnNavigateOutCommandInternalAsync(INavigationParameters navParams = null)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(ChannelUrl);
        }

        async void OnVideoTapped()
        {
            if (VideoSelected != null)
            {
                if (_videoLastSelected != null)
                {
                    _videoLastSelected.ResetColor();
                }

                _videoLastSelected = _videoSelected;

                VideoSelected.Color = "Gray";

                var navParams = new NavigationParameters
                {
                    { NavigationParameterKeys.VideoId, VideoSelected.VideoId },
                };

                await NavigationService.NavigateAsync($"{nameof(PlayVideoPage)}", navParams);

                VideoSelected.ResetColor();
            }
        }

        private async Task OnRefreshAsync()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.None)
            {
                await DialogService.DisplayAlertWithOk("Internet?", "Please check your connection then swipe down to refresh.");
                return;
            }

            try
            {
                if (_firstTime)
                {
                    SeparatorColor = "AliceBlue";
                }

                Videos.Clear();
                Videos = await _youTubeService.GetPlaylistForChannel(ChannelId, 15);
            }
            finally
            {
                if (!_firstTime)
                {
                    _firstTime = true;
                }
            }
        }
    }
}



