using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JimmyDore.Extensions;
using JimmyDore.Models;
using JimmyDore.Services.DialogAlert;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Xamarin.Forms;

namespace JimmyDore.ViewModels
{
    public class PlayVideoViewModel : ViewModelBase
    {
        private DelegateCommand _navigateOutCommand;

        public PlayVideoViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
            PageTitle = "Video Playback";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            VideoId = parameters.Get<string>(NavigationParameterKeys.VideoId);
        }

        string _videoId;
        public string VideoId
        {
            get => _videoId;
            set => SetProperty(ref _videoId, value, OnIdSet);
        }

        private void OnIdSet()
        {
            VideoUrl = $"https://www.youtube.com/embed/{VideoId}";
        }

        string _videoUrl;
        public string VideoUrl
        {
            get => _videoUrl;
            set => SetProperty(ref _videoUrl, value);
        }

        public DelegateCommand NavigateOutCommand => _navigateOutCommand ?? (_navigateOutCommand = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnNavigateOutCommandAsync), CanNavigateOut));

        private bool CanNavigateOut()
        {
            return Xamarin.Essentials.Launcher.CanOpenAsync(VideoUrl).Result;
        }

        private async Task OnNavigateOutCommandAsync()
        {
            await OnNavigateOutCommandInternalAsync();
        }

        protected virtual async Task OnNavigateOutCommandInternalAsync(INavigationParameters navParams = null)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(VideoUrl);
        }
    }
}
