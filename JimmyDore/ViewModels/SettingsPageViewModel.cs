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

        public SettingsPageViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
        }

        public DelegateCommand<string> OnLinkTapped => _onLinkTapped ?? (_onLinkTapped = new DelegateCommand<string>(OnLinkClickAsync));

        private async void OnLinkClickAsync(string url)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(url);
        }
    }
}
