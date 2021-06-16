using System;
using System.Threading.Tasks;
using JimmyDore.Dialogs;

namespace JimmyDore.Services.DialogAlert
{
    public class JimmyDoreDialogService : IJimmyDoreDialogService
    {
        //private readonly IAudioAlertService _audioAlertService;
        //private readonly ILoggingService _loggingService;

        public JimmyDoreDialogService()//IAudioAlertService audioAlertService, ILoggingService loggingService)
        {
            //_audioAlertService = audioAlertService;
            //_loggingService = loggingService;
        }

        public async Task DisplayAlertWithOk(string title, string message, bool withSound = true)
        {
            //_loggingService.Debug("{type} {method} title: {title} message: {dialog_message} withSound:{withSound}", GetType().Name, nameof(DisplayAlertWithOk), title, message, withSound);

            //if (withSound && _audioAlertService != null)
            //{
            //    _audioAlertService.Play(false, NotificationType.Error);
            //}

            var inputView = new AlertDialogView(title, message, "OK");
            await DisplayDialogAsync(inputView);
        }

        public async Task DisplayAlert(string title, string message, string cancelButtonText, bool withSound = true)
        {
            //_loggingService.Debug("{type} {method} title: {title} message: {dialog_message} cancelButtonText: {cancelButtonText} withSound:{withSound}", GetType().Name, nameof(DisplayAlert), title, message, cancelButtonText, withSound);
            //if (withSound && _audioAlertService != null)
            //{
            //    _audioAlertService.Play(false, NotificationType.Error);
            //}

            var inputView = new AlertDialogView(title, message, cancelButtonText);

            await DisplayDialogAsync(inputView);
        }

        public async Task<bool> UserAcceptedDisplayAlert(string title, string message, string acceptButtonText, string cancelButtonText, bool withSound = true)
        {
            //if (withSound && _audioAlertService != null)
            //{
            //    _audioAlertService.Play(false, NotificationType.Error);
            //}

            var inputView = new AlertDialogView(title, message, acceptButtonText, cancelButtonText);
            var response = await DisplayDialogAsync(inputView);
            //_loggingService.Debug("{type} {method} title: {title} message: {dialog_message} cancelButtonText: {cancelButtonText} withSound:{withSound} response: {response}", GetType().Name, nameof(UserAcceptedDisplayAlert), title, message, cancelButtonText, withSound, response);
            return response;

        }


        private async Task<bool> DisplayDialogAsync(AlertDialogView inputView)
        {
            var popup = new AlertDialogBase(inputView);

            // subscribe to the TextInputView's Button click event
            inputView.CancelButtonTapped += OnCancel;
            inputView.AcceptButtonTapped += OnAccept;

            // Push the page to Navigation Stack
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popup);

            // await for the user to enter the text input
            var result = await popup.PageClosedTask;

            // Pop the page from Navigation Stack
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();

            // return user selected result
            return result;
        }

        private void OnAccept(object sender, AlertDialogBase dialog)
        {
            try
            {
                dialog?.PageClosedTaskCompletionSource.TrySetResult(true);

            }
            catch (Exception ex)
            {
                //_loggingService.Debug(ex, "{type} {method}", GetType().Name, nameof(OnAccept));
            }

            if (sender is AlertDialogView inputView)
            {
                inputView.AcceptButtonTapped -= OnAccept;
                inputView.CancelButtonTapped -= OnCancel;
            }
        }

        private void OnCancel(object sender, AlertDialogBase dialog)
        {
            try
            {
                dialog?.PageClosedTaskCompletionSource.TrySetResult(false);

            }
            catch (Exception ex)
            {
                //_loggingService.Debug(ex, "{type} {method}", GetType().Name, nameof(OnCancel));
            }

            if (sender is AlertDialogView inputView)
            {
                inputView.AcceptButtonTapped -= OnAccept;
                inputView.CancelButtonTapped -= OnCancel;
            }
        }
    }
}
