using System;
using System.IO;
using System.Reflection;
using DryIoc;
using JimmyDore.Enums;
using JimmyDore.Event;
using JimmyDore.Pages;
using JimmyDore.ViewModels;
using Prism;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using JimmyDore.Services.DialogAlert;
using JimmyDore.Services.Localise;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using JimmyDore.Service.YouTube;
using System.Net.Http;
using JimmyDore.Service.Podcasts;

namespace JimmyDore
{
    public partial class App : PrismApplication
    {
        public static string PodcastUrl = "https://thejimmydoreshow.libsyn.com/rss";

        private IEventAggregator _eventAggregator;

        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer, true)
        {
        }

        protected override async void OnInitialized()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                PlayBells();
            });

            _eventAggregator = Container.Resolve<IEventAggregator>();

            InitializeComponent();

            SetLocalisation();

            try
            {
                Device.BeginInvokeOnMainThread(async () => await Container.Resolve<IYouTubeService>().GetJimmysVideos(true));

                Device.BeginInvokeOnMainThread(async () => await Container.Resolve<IPodcastService>().RssParse(PodcastUrl));

                await NavigationService.NavigateAsync($"{nameof(CurtainsPage)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            CrossFirebasePushNotification.Current.Subscribe("general");

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
#endif
            };

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Received");
#endif
                Device.BeginInvokeOnMainThread(() => PlayBells());
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += Current_OnNotificationOpened;

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Action");

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
                    foreach (var data in p.Data)
                    {
                        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                    }
                }
#endif
            };
        }

        DateTime dateTime;

        private void Current_OnNotificationOpened(object source, FirebasePushNotificationResponseEventArgs p)
        {
            System.Diagnostics.Debug.WriteLine("Opened");

            var liveNow = false;
            var liveShow = false;

            if (dateTime != default && (DateTime.Now - dateTime).TotalMilliseconds < TimeSpan.FromSeconds(30).TotalMilliseconds)
            {
                return;
            }

            dateTime = DateTime.Now;

            foreach (var data in p.Data)
            {
                if (data.Key.Equals("google.c.a.c_l") && data.Value.Equals("New Show"))
                {
                    liveShow = true;
                }
                if (data.Key.Equals("google.c.a.c_l") && data.Value.Equals("Live Now"))
                {
                    liveNow = true;
                }

                if (data.Key.Equals("aps.alert.body") && liveNow)
                {
                    liveNow = false;

                    Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                    {
                        Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var dialogs = Container.Resolve<IJimmyDoreDialogService>();
                            var answer = await dialogs.UserAcceptedDisplayAlert("Live Show Starting Soon!", $"Open in YouTube now?", "Yes", "No");
                            if (answer)
                            {
                                await Xamarin.Essentials.Launcher.OpenAsync("https://www.youtube.com/channel/UC3M7l8ved_rYQ45AVzS0RGA");
                            }
                        });

                        return false;
                    });
                }

                if (data.Key.Equals("aps.alert.body") && data.Value.ToString().Contains("https") && liveShow)
                {
                    liveShow = false;

                    var parts = data.Value.ToString().Split(new[] { "https" }, StringSplitOptions.None);

                    var link = $"https{parts[1]}";

                    Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                    {
                        Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var dialogs = Container.Resolve<IJimmyDoreDialogService>();
                            var answer = await dialogs.UserAcceptedDisplayAlert("New Live Event Coming!", $"Check it out now?\n\n{link}", "Yes", "No");
                            if (answer)
                            {
                                await Xamarin.Essentials.Launcher.OpenAsync(link);
                            }
                        });

                        return false;
                    });
                }

                System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // service
            containerRegistry.RegisterSingleton<HttpClient>();
            containerRegistry.RegisterSingleton<IYouTubeService, YouTubeService>();
            containerRegistry.RegisterSingleton<IJimmyDoreDialogService, JimmyDoreDialogService>();
            containerRegistry.RegisterSingleton<IPodcastService, PodcastService>();

            // start
            containerRegistry.RegisterForNavigation<CurtainsPage, CurtainsPageViewModel>();
            containerRegistry.RegisterForNavigation<LoadingPage, LoadingPageViewModel>();
            containerRegistry.RegisterForNavigation<RootTabPage, RootTabPageViewModel>();

            // tabs
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<NewsPage, NewsPageViewModel>();
            containerRegistry.RegisterForNavigation<ShowsPage, ShowsPageViewModel>();
            containerRegistry.RegisterForNavigation<PodcastPage, PodcastPageViewModel>();
            containerRegistry.RegisterForNavigation<FavoritesPage, FavoritesPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();

            // navigation
            containerRegistry.RegisterForNavigation<ChannelPage, ChannelPageViewModel>();
            containerRegistry.RegisterForNavigation<PlayVideoPage, PlayVideoViewModel>();
            containerRegistry.RegisterForNavigation<PlayPodcastPage, PlayPodcastViewModel>();
        }

        protected override void OnStart()
        {
            _eventAggregator.GetEvent<AppLifecycleEvent>().Publish(AppLifecycleState.Foreground);
        }

        protected override void OnSleep()
        {
            _eventAggregator.GetEvent<AppLifecycleEvent>().Publish(AppLifecycleState.Background);
        }

        protected override void OnResume()
        {

            _eventAggregator.GetEvent<AppLifecycleEvent>().Publish(AppLifecycleState.Foreground);
        }

        Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;

            var stream = assembly.GetManifestResourceStream("JimmyDore." + filename);

            return stream;
        }

        void SetLocalisation()
        {
            // This lookup NOT required for Windows platforms - the Culture will be automatically set
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS || Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                // determine the correct, supported .NET culture
                var localise = Container.Resolve<ILocalise>();
                var ci = localise.GetCurrentCultureInfo();
                AppResources.Culture = ci;
                localise.SetLocale(ci);
            }
        }

        void PlayBells()
        {
            var stream = GetStreamFromFile("Sounds.jimmydore.mp3");
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(stream);
            audio.Play();
        }
    }
}
