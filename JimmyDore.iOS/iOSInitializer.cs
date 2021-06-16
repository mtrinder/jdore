using JimmyDore.iOS.Services.Localise;
using JimmyDore.Services.Localise;
using Prism.Ioc;

namespace JimmyDore.iOS
{
    public class iOSInitializer : Prism.IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ILocalise, LocaliseIOS>();
        }
    }
}
