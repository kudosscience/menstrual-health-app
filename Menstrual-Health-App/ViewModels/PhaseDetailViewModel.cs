using System.Windows.Input;
using Menstrual_Health_App.Models;
using Menstrual_Health_App.Views;

namespace Menstrual_Health_App.ViewModels;

[QueryProperty(nameof(PhaseInfo), "PhaseInfo")]
public class PhaseDetailViewModel : BaseViewModel
{
    private PhaseInfo _phaseInfo = new();

    public PhaseInfo PhaseInfo
    {
        get => _phaseInfo;
        set
        {
            SetProperty(ref _phaseInfo, value);
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(PhaseColor));
            OnPropertyChanged(nameof(Symptoms));
            OnPropertyChanged(nameof(HealthTips));
            OnPropertyChanged(nameof(NutritionTips));
            OnPropertyChanged(nameof(ExerciseTips));
            OnPropertyChanged(nameof(SelfCareTips));
        }
    }

    public string Title => PhaseInfo.Title;
    public string Description => PhaseInfo.Description;
    public string Icon => PhaseInfo.Icon;
    public Color PhaseColor => PhaseInfo.PhaseColor;
    public List<string> Symptoms => PhaseInfo.Symptoms;
    public List<string> HealthTips => PhaseInfo.HealthTips;
    public List<string> NutritionTips => PhaseInfo.NutritionTips;
    public List<string> ExerciseTips => PhaseInfo.ExerciseTips;
    public List<string> SelfCareTips => PhaseInfo.SelfCareTips;

    public ICommand ViewRecipesCommand { get; }

    public PhaseDetailViewModel()
    {
        ViewRecipesCommand = new Command(async () => await ViewRecipesAsync());
    }

    private async Task ViewRecipesAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(PhaseRecipesPage)}?Phase={PhaseInfo.Phase}");
    }
}
