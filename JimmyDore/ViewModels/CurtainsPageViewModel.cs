using System.Threading.Tasks;
using JimmyDore.Extensions;
using JimmyDore.Pages;
using JimmyDore.Services.DialogAlert;
using Prism.Events;
using Prism.Navigation;

namespace JimmyDore.ViewModels
{
    public class CurtainsPageViewModel : ViewModelBase
    {
        public CurtainsPageViewModel(INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.IsNewNavigation())
            {
                await Task.Delay(4000);
                await NavigationService.NavigateAsync($"/{nameof(LoadingPage)}");
            }
        }
    }
}
