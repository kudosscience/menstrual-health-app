using Menstrual_Health_App.ViewModels;

namespace Menstrual_Health_App.Views;

public partial class SetupPage : ContentPage
{
    public SetupPage(SetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
