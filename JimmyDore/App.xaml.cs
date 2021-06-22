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
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

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
#if DEBUG
            Device.BeginInvokeOnMainThread(() =>
            {
                PlayBells();
            });
#endif
            _eventAggregator = Container.Resolve<IEventAggregator>();

            InitializeComponent();
            SetLocalisation();

            try
            {
#if DEBUG
                await NavigationService.NavigateAsync($"{nameof(CurtainsPage)}");
#else
                await NavigationService.NavigateAsync($"{nameof(LoadingPage)}");
#endif
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

                //Device.BeginInvokeOnMainThread(() =>
                //{
                //    PlayBells();
                //});
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
            containerRegistry.RegisterSingleton<IJimmyDoreDialogService, JimmyDoreDialogService>();

            containerRegistry.RegisterForNavigation<RootTabPage, RootTabPageViewModel>();

            // start
            containerRegistry.RegisterForNavigation<CurtainsPage, CurtainsPageViewModel>();
            containerRegistry.RegisterForNavigation<LoadingPage, LoadingPageViewModel>();

            // tabs
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<NewsPage, NewsPageViewModel>();
            containerRegistry.RegisterForNavigation<ShowsPage, ShowsPageViewModel>();
            containerRegistry.RegisterForNavigation<FavoritesPage, FavoritesPageViewModel>();

            // navigation
            containerRegistry.RegisterForNavigation<PlayVideoPage, PlayVideoViewModel>();

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
