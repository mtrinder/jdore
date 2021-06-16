using Prism.Ioc;
using Prism;
using JimmyDore.Services.Localise;
using JimmyDore.Droid.Services.Localise;

namespace JimmyDore.Droid
{
    public class AndroidInitializer : IPlatformInitializer
    {
        public AndroidInitializer()
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.Register<IAppLaunchService, AppLaunchService>();
            //containerRegistry.Register<IToastAlert, ToastAlert>();
            //containerRegistry.Register<ICloseApplicationService, CloseApplicationService>();
            containerRegistry.Register<ILocalise, LocaliseAndroid>();
            //containerRegistry.Register<IStatusBar, StatusBarImplementation>();
            //containerRegistry.RegisterSingleton<IAudioAlertService, AudioAlertService>();
            //containerRegistry.Register<IConnectionManager, PrintConnectionManager>();
            //containerRegistry.RegisterSingleton<ILoggingService, LoggingService>();
        }
    }
}