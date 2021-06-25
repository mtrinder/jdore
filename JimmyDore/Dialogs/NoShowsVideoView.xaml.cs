using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace JimmyDore.Dialogs
{
    public partial class NoShowsVideoView : ContentView
    {
        // public event handler to expose 
        // the Cancel button's click event
        public event EventHandler<AlertDialogBase> CancelButtonTapped;

        public NoShowsVideoView()
        {
            InitializeComponent();
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            // invoke the event handler if its being subscribed
            var dialog = Parent as AlertDialogBase;
            CancelButtonTapped?.Invoke(this, dialog);
        }
    }
}
