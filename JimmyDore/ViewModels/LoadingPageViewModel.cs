using System;
using System.Threading.Tasks;
using JimmyDore.Extensions;
using JimmyDore.Pages;
using JimmyDore.Services.DialogAlert;
using Prism.Events;
using Prism.Navigation;

namespace JimmyDore.ViewModels
{
    public class LoadingPageViewModel : ViewModelBase
    {
        public LoadingPageViewModel(INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.IsNewNavigation())
            {
                await Task.Delay(2000);
                //await NavigationService.NavigateAsync($"/{nameof(MainPage)}");
            }
        }
    }
}
