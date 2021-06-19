using System.Threading.Tasks;
using JimmyDore.Services.DialogAlert;
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

            //await NavigationService.NavigateAsync($"/{nameof(MasterDetailMenuPage)}/{nameof(TapToStartPage)}");
        }
    }
}
