using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JimmyDore.Extensions;
using JimmyDore.Models;
using JimmyDore.Pages;
using JimmyDore.Service.YouTube;
using JimmyDore.Services.DialogAlert;
using MvvmHelpers;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Xamarin.Forms;

namespace JimmyDore.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        bool _firstTime;
        private readonly IYouTubeService _youTubeService;

        DelegateCommand _listRefresh;
        DelegateCommand _onYouTubeButtonPress;

        public MainPageViewModel(
            INavigationService navigationService,
            IYouTubeService youTubeService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
            _previousSegment = 0;

            _youTubeService = youTubeService;

            SeparatorColor = "DarkBlue";
            
            SegmentStringSource = new[] { "All Videos", "Mike MacRae" }; //The Funny Ones

            MessagingCenter.Subscribe<IYouTubeService>(this, "Video-Retrieve-Failed", s =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsRefreshing = false;
                    SegmentedEnabled = true;
                    SeparatorColor = "DarkBlue";
                });
            });
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.IsNewNavigation())
            {
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

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        bool _segmentEnabled;
        public bool SegmentedEnabled
        {
            get => _segmentEnabled;
            set => SetProperty(ref _segmentEnabled, value);
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

        int _selectedSegment;
        int _previousSegment;
        public int SelectedSegment
        {
            get => _selectedSegment;
            set => SetProperty(ref _selectedSegment, value, ChangeVideoList);
        }

        ObservableRangeCollection<Video> _allVideos = new ObservableRangeCollection<Video>();
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

        string[] _segmentStringSource;
        public string[] SegmentStringSource
        {
            get => _segmentStringSource;
            set => SetProperty(ref _segmentStringSource, value);
        }

        public DelegateCommand ListRefreshCommand => _listRefresh ?? (_listRefresh = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnRefreshAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        public DelegateCommand OnYouTubeButtonPress => _onYouTubeButtonPress ?? (_onYouTubeButtonPress = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnYouTubeAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        private async Task OnYouTubeAsync()
        {
            await Xamarin.Essentials.Launcher.OpenAsync("https://www.youtube.com/channel/UC3M7l8ved_rYQ45AVzS0RGA");
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
                IsRefreshing = false;
                SegmentedEnabled = true;

                await DialogService.DisplayAlertWithOk("Internet?", "Please check your connection then swipe down to refresh.");
                return;
            }

            try
            {
                SelectedSegment = 0;
                _previousSegment = _selectedSegment;

                SegmentedEnabled = false;

                if (_firstTime)
                {
                    SeparatorColor = "AliceBlue";
                }

                var videos = await _youTubeService.GetJimmysVideos(_firstTime);

                _allVideos.Clear();
                _allVideos.AddRange(videos);

                Videos.ReplaceRange(_allVideos);

                ChangeVideoList();

                IsRefreshing = false;
                SegmentedEnabled = true;
                SeparatorColor = "DarkBlue";
            }
            finally
            {
                if (!_firstTime)
                {
                    _firstTime = true;
                }
            }
        }

        void ChangeVideoList()
        {
            if (_previousSegment == _selectedSegment)
            {
                return;
            }

            _previousSegment = _selectedSegment;

            Device.BeginInvokeOnMainThread(() =>
            {
                switch (_selectedSegment)
                {
                    case 0:
                        Videos.ReplaceRange(_allVideos);
                        break;
                    case 1:
                        Videos.ReplaceRange(_allVideos.Where(x => x.Funny));
                        break;
                }
            });
        }
    }
}



