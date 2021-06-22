using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JimmyDore.Extensions;
using JimmyDore.Models;
using JimmyDore.Pages;
using JimmyDore.Services.DialogAlert;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Xamarin.Forms;

namespace JimmyDore.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        YouTubeResult _videosResult;
        DelegateCommand _listRefresh;
        DelegateCommand _onYouTubeButtonPress;

        HttpClient httpClient = new HttpClient();

        public MainPageViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
        }

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        bool _showList;
        public bool ShowList
        {
            get => _showList;
            set => SetProperty(ref _showList, value);
        }

        int _selectedSegment;
        public int SelectedSegment
        {
            get => _selectedSegment;
            set => SetProperty(ref _selectedSegment, value, ChangeVideoList);
        }

        ObservableCollection<Video> _videos = new ObservableCollection<Video>();
        public ObservableCollection<Video> Videos
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

        public string[] SegmentStringSource => new[] { "All Videos", "Editorial", "Comedy" };

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
                    { "VideoId", VideoSelected.VidId },
                };

                await NavigationService.NavigateAsync($"{nameof(PlayVideoPage)}", navParams);

                VideoSelected.ResetColor();
            }
        }

        public DelegateCommand ListRefreshCommand => _listRefresh ?? (_listRefresh = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnRefreshAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        public DelegateCommand OnYouTubeButtonPress => _onYouTubeButtonPress ?? (_onYouTubeButtonPress = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnYouTubeAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        private async Task OnYouTubeAsync()
        {
            await Xamarin.Essentials.Launcher.OpenAsync("https://www.youtube.com/channel/UC3M7l8ved_rYQ45AVzS0RGA");
        }

        bool _firstTime;
        private async Task OnRefreshAsync()
        {
            try
            {
                if (_firstTime)
                {
                    IsRefreshing = true;
                }

                var result = await httpClient.GetStringAsync("https://youtube.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=100&playlistId=UU3M7l8ved_rYQ45AVzS0RGA&key=AIzaSyBhgYXnsRrKfbgDBabmGUjzf8Wt-AtcZLo");

                _videosResult = JsonConvert.DeserializeObject<YouTubeResult>(result);

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    Task.Run(async () =>
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            foreach (var item in _videosResult.Items)
                            {
                                result = await httpClient.GetStringAsync($"https://youtube.googleapis.com/youtube/v3/videos?part=statistics&id={item.Snippet.ResourceId.VideoId}&key=AIzaSyBhgYXnsRrKfbgDBabmGUjzf8Wt-AtcZLo");

                                var stats = JsonConvert.DeserializeObject<VideoResult>(result);

                                item.Snippet.ResourceId.LikeCount = (int.Parse(stats.Items[0].Statistics.LikeCount) / 1000).ToString("0.#k") + " Likes";
                                item.Snippet.ResourceId.ViewCount = (int.Parse(stats.Items[0].Statistics.ViewCount) / 1000).ToString("0.#k") + " Views";

                                var element = _videos.FirstOrDefault(x => x.VidId.Equals(item.Snippet.ResourceId.VideoId));

                                if (element != null)
                                {
                                    element.Likes = item.Snippet.ResourceId.LikeCount;
                                    element.Views = item.Snippet.ResourceId.ViewCount;
                                }

                                var index = _videos.IndexOf(element);

                                _videos[index] = element.Clone() as Video;

                            }
                        }).ConfigureAwait(false);
                    });
                    return false;
                });

                ChangeVideoList();
            }
            finally
            {
                if (!_firstTime)
                {
                    _firstTime = true;
                }
                IsRefreshing = false;
            }
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

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Device.BeginInvokeOnMainThread(() => ShowList = true);
                }
            }
        }

        void ChangeVideoList()
        {
            System.Collections.Generic.List<Item> vids = _videosResult.Items;

            _videos.Clear();

            switch (_selectedSegment)
            {
                case 1:
                    vids = _videosResult.Items.Where(x => !x.Snippet.Description.Contains("Performed by Mike MacRae")).ToList();
                    break;
                case 2:
                    vids = _videosResult.Items.Where(x => x.Snippet.Description.Contains("Performed by Mike MacRae")).ToList();
                    break;
            }

            foreach (var item in vids)
            {
                var video = new Video
                {
                    Title = item.Snippet.Title + "                    ",
                    Link = item.Snippet.Thumbnails.Medium.Url,
                    VidId = item.Snippet.ResourceId.VideoId,
                    Likes = item.Snippet.ResourceId.LikeCount,
                    Views = item.Snippet.ResourceId.ViewCount
                };

                _videos.Add(video);
            }
        }
    }
}



