using System;
using System.Threading.Tasks;
using JimmyDore.Services.DialogAlert;
using Plugin.FirebasePushNotification;
using Prism.Events;
using Prism.Navigation;

namespace JimmyDore.ViewModels
{
    public class RootTabPageViewModel : ViewModelBase
    {
        public RootTabPageViewModel(INavigationService navigationService, IJimmyDoreDialogService dialogService, IEventAggregator eventAggregator)
            : base(navigationService, dialogService, eventAggregator)
        {
            PageTitle = "Jimmy Dore Show";
        }

        protected override async Task OnCloseInternalAsync()
        {
            await base.OnCloseInternalAsync();
        }
    }
}
