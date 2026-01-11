using System.Windows.Input;
using Menstrual_Health_App.Models;
using Menstrual_Health_App.Services;

namespace Menstrual_Health_App.ViewModels;

[QueryProperty(nameof(MealId), "MealId")]
public class RecipeDetailViewModel : BaseViewModel
{
    private readonly RecipeService _recipeService;

    private string _mealId = string.Empty;
    private Meal? _meal;
    private bool _isLoading;
    private bool _hasError;

    public string MealId
    {
        get => _mealId;
        set
        {
            if (SetProperty(ref _mealId, value))
            {
                LoadMealAsync();
            }
        }
    }

    public Meal? Meal
    {
        get => _meal;
        set
        {
            if (SetProperty(ref _meal, value))
            {
                OnPropertyChanged(nameof(MealName));
                OnPropertyChanged(nameof(MealImage));
                OnPropertyChanged(nameof(Category));
                OnPropertyChanged(nameof(Area));
                OnPropertyChanged(nameof(Instructions));
                OnPropertyChanged(nameof(Ingredients));
                OnPropertyChanged(nameof(HasYoutubeLink));
                OnPropertyChanged(nameof(YoutubeLink));
            }
        }
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

    public string MealName => Meal?.StrMeal ?? "Loading...";
    public string MealImage => Meal?.StrMealThumb ?? string.Empty;
    public string Category => Meal?.StrCategory ?? string.Empty;
    public string Area => Meal?.StrArea ?? string.Empty;
    public string Instructions => Meal?.StrInstructions ?? string.Empty;
    public List<string> Ingredients => Meal?.GetIngredientsList() ?? new List<string>();
    public bool HasYoutubeLink => !string.IsNullOrWhiteSpace(Meal?.StrYoutube);
    public string YoutubeLink => Meal?.StrYoutube ?? string.Empty;

    public ICommand OpenYoutubeCommand { get; }
    public ICommand RetryCommand { get; }

    public RecipeDetailViewModel(RecipeService recipeService)
    {
        _recipeService = recipeService;
        OpenYoutubeCommand = new Command(async () => await OpenYoutubeAsync());
        RetryCommand = new Command(async () => await LoadMealAsync());
    }

    private async Task LoadMealAsync()
    {
        if (string.IsNullOrEmpty(MealId))
            return;

        IsLoading = true;
        HasError = false;

        try
        {
            Meal = await _recipeService.GetMealByIdAsync(MealId);
            HasError = Meal == null;
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

    private async Task OpenYoutubeAsync()
    {
        if (HasYoutubeLink)
        {
            try
            {
                await Launcher.OpenAsync(new Uri(YoutubeLink));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening YouTube: {ex.Message}");
            }
        }
    }
}
