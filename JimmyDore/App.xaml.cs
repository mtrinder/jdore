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

namespace JimmyDore
{
    public partial class App : PrismApplication
    {
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
                Device.BeginInvokeOnMainThread(() =>
                {
                    PlayBells();

                    //IJimmyDoreDialogService DialogService;
                    //DialogService = Container.Resolve<IJimmyDoreDialogService>();
                    //await DialogService.DisplayAlertWithOk("The Jimmy Dore Show", "We are now live on YouTube!");
                });

                
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Opened");
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }
#endif
            };

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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // service
            containerRegistry.RegisterSingleton<HttpClient>();
            containerRegistry.RegisterSingleton<IYouTubeService, YouTubeService>();
            containerRegistry.RegisterSingleton<IJimmyDoreDialogService, JimmyDoreDialogService>();

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
