using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
//using JimmyDore.Events;
using JimmyDore.Extensions;
using JimmyDore.Services.DialogAlert;
using Prism;
//using JimmyDore.Services.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
//using JimmyDore.Models;

namespace JimmyDore.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible, IActiveAware
    {
        protected INavigationService NavigationService { get; }
        protected IEventAggregator EventAggregator { get; }
        //protected ILoggingService LoggingService { get; }
        protected IJimmyDoreDialogService DialogService { get; }
        private string _pageTitle;
        private bool _isBusy;
        private bool _isLocked;
        private int _commandLock;
        private Task _executeCommandTask;
        private bool _isDetecting;
        private DelegateCommand _navigateBackCommand;
        private DelegateCommand _closeCommand;
        private DelegateCommand _toggleMasterPageCommand;
        private bool _isVisibleBackButton = true;

        public event EventHandler IsActiveChanged;

        bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnActiveChanged(_isActive);
            }
        }

        bool _actionOutVisible;
        public virtual bool ActionOutVisible
        {
            get => _actionOutVisible;
            set => SetProperty(ref _actionOutVisible, value);
        }

        protected virtual void OnActiveChanged(bool active)
        {

        }

        protected ViewModelBase(INavigationService navigationService, IJimmyDoreDialogService dialogService, IEventAggregator eventAggregator)//, ILoggingService loggingService)
        {
            //loggingService.Trace("{type} {method}", GetType().Name, ".ctor");
            ActionOutVisible = true;
            NavigationService = navigationService;
            DialogService = dialogService;
            EventAggregator = eventAggregator;
            //LoggingService = loggingService;
        }

        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool IsVisibleBackButton
        {
            get => _isVisibleBackButton;
            set => SetProperty(ref _isVisibleBackButton, value);
        }

        /// <summary>
        /// Returns true if a task is currently executing in the locking block
        /// </summary>
        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }
        public DelegateCommand ToggleMasterPageCommand =>
            _toggleMasterPageCommand ?? (_toggleMasterPageCommand = new DelegateCommand(ExecuteToggleMasterPageCommand));

        void ExecuteToggleMasterPageCommand()
        {
            //EventAggregator.GetEvent<ToggleMenuEvent>().Publish();
        }

        public DelegateCommand CloseCommand => _closeCommand ?? (_closeCommand = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnCloseCommandAsync)));

        private async Task OnCloseCommandAsync()
        {
            await OnCloseInternalAsync();
        }

        protected virtual Task OnCloseInternalAsync()
        {
            return Task.CompletedTask;
        }

        public DelegateCommand NavigateBackCommand => _navigateBackCommand ?? (_navigateBackCommand = new DelegateCommand(async () => await ExecuteTaskInLockAsync(OnNavigateBackCommandAsync), CanNavigateBack));

        private async Task OnNavigateBackCommandAsync()
        {
            await OnNavigateBackCommandInternalAsync();
        }

        protected virtual async Task OnNavigateBackCommandInternalAsync(INavigationParameters navParams = null)
        {
            await NavigationService.GoBackAsync(navParams);
        }

        private bool CanNavigateBack()
        {
            return CanExecute();
        }
        
        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            //LoggingService.Trace("{type} {method} parameters: {parameters}", GetType().Name, nameof(OnNavigatedFrom), parameters);
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            // NavigationService can be a INavigationServiceProxy during Unit Tests
            var isNew = parameters.IsNewNavigation();
            if (NavigationService is Prism.Common.IPageAware)
            {
                //LoggingService.Trace("{type} {method} {navMode} parameters: {parameters} path: {path}", GetType().Name, nameof(OnNavigatedTo), isNew ? "NEW": "BACK", parameters, NavigationService.GetNavigationUriPath());
            }
            else
            {
                //LoggingService.Trace("{type} {method} parameters: {parameters}", GetType().Name, nameof(OnNavigatedTo), parameters);
            }
        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
            if (NavigationService is Prism.Common.IPageAware)
            {
                //LoggingService.Trace("{type} {method} parameters: {parameters} path: {path}", GetType().Name, nameof(OnNavigatingTo), parameters, NavigationService.GetNavigationUriPath());
            }
            else
            {
                //LoggingService.Trace("{type} {method} parameters: {parameters}", GetType().Name, nameof(OnNavigatingTo), parameters);
            }
        }

        public virtual void Destroy()
        {
            //LoggingService.Trace("{type} {method}", GetType().Name, nameof(Destroy));

        }
        // Alerting Methods

        public async Task DisplayNoShows()
        {
            if (DialogService != null)
            {
                await DialogService.DisplayNoShows();
            }
        }

        public async Task DisplayAlertWithOk(string title, string message)
        {
            if (DialogService != null)
            {
                await DialogService.DisplayAlertWithOk(title, message);
            }
        }

        public async Task DisplayAlert(string title, string message, string cancelButtonText)
        {
            if (DialogService != null)
            {
                await DialogService.DisplayAlert(title, message, cancelButtonText);
            }
        }

        public async Task<bool> UserAcceptedDisplayAlert(string title, string message, string acceptButtonText, string cancelButtonText)
        {
            if (DialogService != null)
            {
                return await DialogService.UserAcceptedDisplayAlert(title, message, acceptButtonText, cancelButtonText);
            }
            return false;
        }

        /// <summary>
        /// Returns true if commands can execute. Can be called from DelegateCommand CanExecute methods.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanExecute()
        {
            return !IsBusy && !IsLocked;
        }

        /// <summary>
        /// Runs the task within a thread safe lock. Aborts the task if another task is current running inside the lock.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        protected async Task ExecuteTaskInLockAsync(Func<Task> command, [CallerMemberName] string caller = "")
        {
            if (Interlocked.CompareExchange(ref (_commandLock), 1, 0) != 0)
            {
                // already have a running task
                //LoggingService.Debug("{type} {method} caller: {caller} running in other thread. Aborting.", GetType().Name, nameof(ExecuteTaskInLockAsync), caller);
                return;
            }
            IsLocked = true;

            if (_executeCommandTask?.IsCompleted ?? true)
            {
                _executeCommandTask = command.Invoke();
            }

            try
            {
                await _executeCommandTask;
            }
            finally
            {
                IsLocked = false;

                Interlocked.Exchange(ref _commandLock, 0);
            }
        }
    }
}
