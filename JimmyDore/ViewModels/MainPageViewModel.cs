using JimmyDore.Extensions;
using JimmyDore.Services.DialogAlert;
using Prism.Events;
using Prism.Navigation;

namespace JimmyDore.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.IsNewNavigation())
            {
            }
            else
            {
            }
        }
    }
}
