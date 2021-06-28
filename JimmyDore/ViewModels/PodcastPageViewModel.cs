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
    public class PodcastPageViewModel : ViewModelBase
    {
        bool _active;

        DelegateCommand _listRefresh;
        DelegateCommand _onMicButtonPress;

        public PodcastPageViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
            SeparatorColor = "DarkBlue";
        }

        string _url = "https://thejimmydoreshow.libsyn.com/rss";

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
                    IsRefreshing = true;
                    var pods = await RssParse(_url);
                    Podcasts.AddRange(pods);
                }
                finally
                {
                    IsRefreshing = false;
                    SeparatorColor = "DarkBlue";
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
                var pods = await RssParse(_url);

                Podcasts.Clear();

                Podcasts.AddRange(pods);
            }
            finally
            {
                IsRefreshing = false;
                SeparatorColor = "DarkBlue";
            }
        }

        public async Task<List<Podcast>> RssParse(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var stream = await client.GetStreamAsync(url);

                    var items = await FeedParser.ParseAsync(stream,
                        xDocument => xDocument.Root
                            .Descendants()
                            .Where(i => i.Name.LocalName == "channel")
                            .Elements()
                            .Where(i => i.Name.LocalName == "item"),
                        item => new Podcast
                        {
                            Title = item.GetElementValue<string>("title"),
                            Date = item.GetElementValue<DateTime>("pubDate"),
                            Description = item.GetElementValue<string>("description"),
                            Link = (item.Nodes().ToList().FirstOrDefault(n => n.ToString().Contains("enclosure")) as XElement).Attribute("url").Value,
                        });

                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new List<Podcast>();
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
