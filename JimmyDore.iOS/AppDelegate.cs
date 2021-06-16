using System.Threading.Tasks;
using Foundation;
using NControl.iOS;
using Prism.Events;
using Lottie.Forms.Platforms.Ios;
using Plugin.Permissions;
using Prism.Ioc;
using Xamarin.Forms;
using UIKit;

namespace JimmyDore.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        App _app;

        IEventAggregator _eventAggregator;

        public override bool FinishedLaunching(UIApplication application, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            Plugin.InputKit.Platforms.iOS.Config.Init();

            NControlViewRenderer.Init();

            Rg.Plugins.Popup.Popup.Init();

            LoadApplication(_app = new App(new iOSInitializer()));

            _eventAggregator = _app.Container.Resolve<IEventAggregator>();

            return base.FinishedLaunching(application, options);
        }

        [Export("applicationWillEnterForeground:")]
        public override void WillEnterForeground(UIApplication application)
        {
            
        }
    }
}
