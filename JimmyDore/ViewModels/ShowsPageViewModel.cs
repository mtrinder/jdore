using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using FeedParserCore;
using JimmyDore.Extensions;
using JimmyDore.Models;
using JimmyDore.Pages;
using JimmyDore.Services.DialogAlert;
using MvvmHelpers;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Xamarin.Forms;

namespace JimmyDore.ViewModels
{
    public class ShowsPageViewModel : ViewModelBase
    {
        bool _active;

        DelegateCommand _listRefresh;
        DelegateCommand _onShowsButtonPress;

        public ShowsPageViewModel(
            INavigationService navigationService,
            IJimmyDoreDialogService dialogService,
            IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
        {
        }

        string _separatorColor;
        public string SeparatorColor
        {
            get => _separatorColor;
            set => SetProperty(ref _separatorColor, value);
        }

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public DelegateCommand OnShowsButtonPress => _onShowsButtonPress ?? (_onShowsButtonPress = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnViewShowsAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        private async Task OnViewShowsAsync()
        {
            await Xamarin.Essentials.Launcher.OpenAsync("https://jimmydorecomedy.com/events/list/");
        }

        public DelegateCommand ListRefreshCommand => _listRefresh ?? (_listRefresh = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnRefreshAsync), CanExecute)
            .ObservesProperty(() => IsBusy).ObservesProperty(() => IsLocked));

        private async Task OnRefreshAsync()
        {
            _active = false;

            SeparatorColor = "AliceBlue";

            try
            {
                IsRefreshing = true;

                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            finally
            {
                IsRefreshing = false;

                await DialogService.DisplayNoShows();
            }
        }

        protected override async void OnActiveChanged(bool active)
        {
            if (!_active)
            {
                _active = true;

                if (Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.None)
                {
                    await DialogService.DisplayAlertWithOk("Internet?", "Please check your connection then swipe down to refresh.");
                    return;
                }

                SeparatorColor = "AliceBlue";

                //try
                //{
                //    IsRefreshing = true;

                //    await Task.Delay(TimeSpan.FromSeconds(4));
                //}
                //finally
                //{
                //    IsRefreshing = false;

                //    await DialogService.DisplayNoShows();
                //}
            }
        }
    }
}
