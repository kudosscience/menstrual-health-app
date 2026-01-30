using Menstrual_Health_App.ViewModels;

namespace Menstrual_Health_App.Views;

public partial class PhaseRecipesPage : ContentPage
{
    public PhaseRecipesPage(PhaseRecipesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
