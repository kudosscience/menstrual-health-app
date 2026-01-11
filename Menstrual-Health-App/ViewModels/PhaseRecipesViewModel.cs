using System.Collections.ObjectModel;
using System.Windows.Input;
using Menstrual_Health_App.Models;
using Menstrual_Health_App.Services;
using Menstrual_Health_App.Views;

namespace Menstrual_Health_App.ViewModels;

[QueryProperty(nameof(Phase), "Phase")]
public class PhaseRecipesViewModel : BaseViewModel
{
    private readonly RecipeService _recipeService;

    private CyclePhase _phase;
    private bool _isLoading;
    private bool _hasError;
    private string _nutritionReason = string.Empty;

    public CyclePhase Phase
    {
        get => _phase;
        set
        {
            if (SetProperty(ref _phase, value))
            {
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(PhaseColor));
                NutritionReason = RecipeService.GetPhaseNutritionReason(value);
                LoadRecipesAsync();
            }
        }
    }

    public string Title => $"{Phase} Phase Recipes";

    public Color PhaseColor => Phase switch
    {
        CyclePhase.Menstrual => Color.FromArgb("#E74C3C"),
        CyclePhase.Follicular => Color.FromArgb("#E91E63"),
        CyclePhase.Ovulation => Color.FromArgb("#FF9800"),
        CyclePhase.Luteal => Color.FromArgb("#9C27B0"),
        _ => Colors.Gray
    };

    public string NutritionReason
    {
        get => _nutritionReason;
        set => SetProperty(ref _nutritionReason, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    public ObservableCollection<MealSummary> Recipes { get; } = new();

    public ICommand SelectRecipeCommand { get; }
    public ICommand RetryCommand { get; }

    public PhaseRecipesViewModel(RecipeService recipeService)
    {
        _recipeService = recipeService;
        SelectRecipeCommand = new Command<MealSummary>(async (meal) => await SelectRecipeAsync(meal));
        RetryCommand = new Command(async () => await LoadRecipesAsync());
    }

    private async Task LoadRecipesAsync()
    {
        IsLoading = true;
        HasError = false;
        Recipes.Clear();

        try
        {
            var meals = await _recipeService.GetRecommendedMealsForPhaseAsync(Phase);
            
            foreach (var meal in meals)
            {
                Recipes.Add(meal);
            }

            HasError = Recipes.Count == 0;
        }
        catch
        {
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SelectRecipeAsync(MealSummary? meal)
    {
        if (meal == null)
            return;

        await Shell.Current.GoToAsync($"{nameof(RecipeDetailPage)}?MealId={meal.IdMeal}");
    }
}
