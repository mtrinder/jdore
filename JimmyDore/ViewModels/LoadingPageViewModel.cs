using System;
using System.Threading.Tasks;
using JimmyDore.Extensions;
using JimmyDore.Pages;
using JimmyDore.Services.DialogAlert;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;

namespace JimmyDore.ViewModels
{
    public class LoadingPageViewModel : ViewModelBase
    {
        DelegateCommand _accessMember;
        DelegateCommand _accessGuest;

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
#if !DEBUG
                await NavigationService.NavigateAsync($"/{nameof(RootTabPage)}");
#endif
            }
        }

        public DelegateCommand MemberAccessCommand => _accessMember ?? (_accessMember = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnPressMemeberAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        public DelegateCommand GuestAccessCommand => _accessGuest ?? (_accessGuest = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnPressGuestAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        private async Task OnPressGuestAsync()
        {
            await EnterAsGuestAsync();
        }

        private async Task OnPressMemeberAsync()
        {
            await EnterAsMemberAsync();
        }

        private async Task EnterAsGuestAsync()
        {
            await NavigationService.NavigateAsync($"/{nameof(RootTabPage)}");
        }

        private async Task EnterAsMemberAsync()
        {
            await NavigationService.NavigateAsync($"/{nameof(RootTabPage)}");
        }
    }
}