using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace JimmyDore.Dialogs
{
    public class AlertDialogBase : PopupPage
    {
        // the awaitable task
        public Task<bool> PageClosedTask => PageClosedTaskCompletionSource.Task;

        // the task completion source
        public TaskCompletionSource<bool> PageClosedTaskCompletionSource { get; set; }

        public AlertDialogBase(View contentBody)
        {
            Content = contentBody;

            // init the task completion source
            PageClosedTaskCompletionSource = new System.Threading.Tasks.TaskCompletionSource<bool>();

            BackgroundColor = new Color(0, 0, 0, 0.4);
        }

        // Method for animation child in PopupPage
        // Invoked after custom animation end
        protected override void OnAppearingAnimationEnd()
        {
            Content.FadeTo(1);
        }

        // Method for animation child in PopupPage
        // Invoked before custom animation begin
        protected override void OnDisappearingAnimationBegin()
        {
            Content.FadeTo(1);
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent back button pressed action on android
            return true;
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Prevent background clicked action
            return false;
        }
    }
}