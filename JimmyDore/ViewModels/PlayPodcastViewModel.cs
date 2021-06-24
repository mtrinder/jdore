using System.Threading.Tasks;
using JimmyDore.Extensions;
using JimmyDore.Services.DialogAlert;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;

namespace JimmyDore.ViewModels
{
    public class PlayPodcastViewModel : ViewModelBase
    {
        private DelegateCommand _navigateOutCommand;

        public PlayPodcastViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
            PageTitle = "Audio Playback";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            PodcastLink = parameters.Get<string>(NavigationParameterKeys.PodcastLink);
        }

        string _podcastLink;
        public string PodcastLink
        {
            get => _podcastLink;
            set => SetProperty(ref _podcastLink, value, OnIdSet);
        }

        private void OnIdSet()
        {
            AudioUrl = PodcastLink;
        }

        string _audioUrl;
        public string AudioUrl
        {
            get => _audioUrl;
            set => SetProperty(ref _audioUrl, value);
        }

        public DelegateCommand NavigateOutCommand => _navigateOutCommand ?? (_navigateOutCommand = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnNavigateOutCommandAsync), CanNavigateOut));

        private bool CanNavigateOut()
        {
            return Xamarin.Essentials.Launcher.CanOpenAsync(AudioUrl).Result;
        }

        private async Task OnNavigateOutCommandAsync()
        {
            await OnNavigateOutCommandInternalAsync();
        }

        protected virtual async Task OnNavigateOutCommandInternalAsync(INavigationParameters navParams = null)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(AudioUrl);
        }
    }
}
