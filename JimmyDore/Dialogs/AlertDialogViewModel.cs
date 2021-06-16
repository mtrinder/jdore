using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace JimmyDore.Dialogs
{
    public class AlertDialogViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService _navigationService;

        public AlertDialogViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty( ref _message, value ); }
        }


        public void OnNavigatingTo(INavigationParameters parameters)
        {
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {

        }

    }
}
