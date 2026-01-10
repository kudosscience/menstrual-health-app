using Menstrual_Health_App.ViewModels;

namespace Menstrual_Health_App
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MainViewModel vm)
            {
                await vm.RefreshDataAsync();
            }
        }
    }
}
