using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.CurrentActivity;
using NControl.Droid;
using Android.Views;
using Lottie.Forms.Platforms.Android;
using Plugin.Permissions;
using Prism.Events;
using Prism.Ioc;
using Xamarin.Forms;
using View = Android.Views.View;

namespace JimmyDore.Droid
{
    [Activity(
        Label = "JimmyDore",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private IEventAggregator _eventAggregator;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            // Disable fast renderers due to crash in release mode
            Forms.SetFlags("UseLegacyRenderers");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Plugin.InputKit.Platforms.Droid.Config.Init(this, savedInstanceState);
            NControlViewRenderer.Init();
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this);

            var app = new App(new AndroidInitializer());

            _eventAggregator = app.Container.Resolve<IEventAggregator>();

            LoadApplication(app);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}