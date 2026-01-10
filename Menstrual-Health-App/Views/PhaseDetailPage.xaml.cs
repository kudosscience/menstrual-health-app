using Menstrual_Health_App.ViewModels;

namespace Menstrual_Health_App.Views;

public partial class PhaseDetailPage : ContentPage
{
    public PhaseDetailPage(PhaseDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
