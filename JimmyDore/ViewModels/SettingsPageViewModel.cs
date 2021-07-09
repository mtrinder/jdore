using System.Threading.Tasks;
using JimmyDore.Extensions;
using JimmyDore.Services.DialogAlert;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;

namespace JimmyDore.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        DelegateCommand<string> _onLinkTapped;
        DelegateCommand _settingsCloseButtonPress;

        public SettingsPageViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
        }

        public DelegateCommand OnSettingsCloseButtonPress => _settingsCloseButtonPress ?? (_settingsCloseButtonPress = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnYouTubeAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        private async Task OnYouTubeAsync()
        {
            await NavigationService.GoBackAsync();
        }

        public DelegateCommand<string> OnLinkTapped => _onLinkTapped ?? (_onLinkTapped = new DelegateCommand<string>(OnLinkClickAsync));

        private async void OnLinkClickAsync(string url)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(url);
        }
    }
}
