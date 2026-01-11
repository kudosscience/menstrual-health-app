using Menstrual_Health_App.ViewModels;

namespace Menstrual_Health_App.Views;

public partial class RecipeDetailPage : ContentPage
{
    public RecipeDetailPage(RecipeDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
