using JimmyDore.Event;
using Prism.Events;
using Xamarin.Forms;

namespace JimmyDore.Pages
{
    public partial class RootTabPage : TabbedPage
    {
        private readonly IEventAggregator _eventAggregator;

        public RootTabPage(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent system back navigation
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _eventAggregator.GetEvent<ChangeTabEvent>().Subscribe(OnChangeTab);
        }

        private void OnChangeTab(string page)
        {
            switch (page)
            {
                case nameof(MainPage):
                    if (Children?.Count > 0) CurrentPage = Children[0];
                    break;
                //case nameof(PassRootPage):
                //    if (Children?.Count > 1) CurrentPage = Children[1];
                //    break;
            }
        }

    }
}
