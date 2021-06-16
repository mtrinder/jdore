using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace JimmyDore.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlertDialogView : ContentView
    {

        public AlertDialogView(string titleText, string messageText, string acceptButtonText, string cancelButtonText)
        {
            InitializeComponent();

            // update the Element's textual values
            TitleLabel.Text = titleText;
            MessageLabel.Text = messageText;
            AcceptButton.Text = acceptButtonText;
            CancelButton.Text = cancelButtonText;

        }

        public AlertDialogView(string titleText, string messageText, string cancelButtonText)
        {
            InitializeComponent();

            // update the Element's textual values
            TitleLabel.Text = titleText;
            MessageLabel.Text = messageText;
            CancelButton.Text = cancelButtonText;
            AcceptButton.IsVisible = false;

        }


        // public event handler to expose 
	    // the Save button's click event
        public event EventHandler<AlertDialogBase> AcceptButtonTapped;

	    // public event handler to expose 
	    // the Cancel button's click event
        public event EventHandler<AlertDialogBase> CancelButtonTapped;


	    private void OnAcceptButtonClicked(object sender, EventArgs e)
	    {
            // invoke the event handler if its being subscribed
            var dialog = Parent as AlertDialogBase;
            AcceptButtonTapped?.Invoke(this, dialog);
        }

	    private void OnCancelButtonClicked(object sender, EventArgs e)
	    {
            // invoke the event handler if its being subscribed
            var dialog = Parent as AlertDialogBase;
            CancelButtonTapped?.Invoke(this, dialog);
	    }
    }
}