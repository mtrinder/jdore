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
#if !DEBUG
            var stream = GetStreamFromFile("Sounds.jimmydore.mp3");
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(stream);
            audio.Play();
#endif
            _eventAggregator = Container.Resolve<IEventAggregator>();

            InitializeComponent();
            SetLocalisation();


            try
            {
#if !DEBUG
                await NavigationService.NavigateAsync($"{nameof(CurtainsPage)}");
#else
                await NavigationService.NavigateAsync($"{nameof(LoadingPage)}");
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IJimmyDoreDialogService, JimmyDoreDialogService>();

            containerRegistry.RegisterForNavigation<RootTabPage, RootTabPageViewModel>();
            containerRegistry.RegisterForNavigation<CurtainsPage, CurtainsPageViewModel>();
            containerRegistry.RegisterForNavigation<LoadingPage, LoadingPageViewModel>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
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

        private void SetLocalisation()
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
    }
}
