using System.Net.Http.Json;
using Menstrual_Health_App.Models;

namespace Menstrual_Health_App.Services;

/// <summary>
/// Service for fetching recipes from TheMealDB API
/// </summary>
public class RecipeService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://www.themealdb.com/api/json/v1/1";

    public RecipeService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Gets recommended search terms for each cycle phase based on nutritional needs
    /// </summary>
    public static List<string> GetPhaseSearchTerms(CyclePhase phase)
    {
        return phase switch
        {
            // Menstrual: Iron-rich foods, anti-inflammatory, comfort foods
            CyclePhase.Menstrual => new List<string> { "beef", "spinach", "salmon", "chocolate", "soup" },
            
            // Follicular: Light, fresh foods, fermented foods, lean proteins
            CyclePhase.Follicular => new List<string> { "chicken", "salad", "avocado", "egg", "fish" },
            
            // Ovulation: Antioxidant-rich, light energizing meals
            CyclePhase.Ovulation => new List<string> { "seafood", "vegetable", "fruit", "quinoa", "shrimp" },
            
            // Luteal: Complex carbs, magnesium-rich foods, comfort foods
            CyclePhase.Luteal => new List<string> { "pasta", "potato", "rice", "banana", "oatmeal" },
            
            _ => new List<string> { "healthy", "chicken", "vegetable" }
        };
    }

    /// <summary>
    /// Gets a description of why certain foods are recommended for each phase
    /// </summary>
    public static string GetPhaseNutritionReason(CyclePhase phase)
    {
        return phase switch
        {
            CyclePhase.Menstrual => "Iron-rich foods help replenish what's lost during menstruation. Anti-inflammatory foods can help reduce cramps and discomfort.",
            CyclePhase.Follicular => "Light, fresh foods support rising energy levels. Lean proteins and fermented foods promote gut health and hormone balance.",
            CyclePhase.Ovulation => "Antioxidant-rich foods support peak energy. Light meals with zinc and B-vitamins help maintain vitality.",
            CyclePhase.Luteal => "Complex carbohydrates boost serotonin levels. Magnesium-rich foods help reduce PMS symptoms and improve mood.",
            _ => "Balanced nutrition supports overall cycle health."
        };
    }

    /// <summary>
    /// Searches for meals by ingredient
    /// </summary>
    public async Task<List<MealSummary>> SearchMealsByIngredientAsync(string ingredient)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<MealSearchResponse>(
                $"{BaseUrl}/filter.php?i={Uri.EscapeDataString(ingredient)}");
            
            return response?.Meals ?? new List<MealSummary>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error searching meals: {ex.Message}");
            return new List<MealSummary>();
        }
    }

    /// <summary>
    /// Gets full meal details by ID
    /// </summary>
    public async Task<Meal?> GetMealByIdAsync(string mealId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<MealResponse>(
                $"{BaseUrl}/lookup.php?i={mealId}");
            
            return response?.Meals?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting meal details: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets recommended meals for a specific cycle phase
    /// </summary>
    public async Task<List<MealSummary>> GetRecommendedMealsForPhaseAsync(CyclePhase phase, int maxResults = 6)
    {
        var searchTerms = GetPhaseSearchTerms(phase);
        var allMeals = new List<MealSummary>();
        var seenIds = new HashSet<string>();

        foreach (var term in searchTerms)
        {
            if (allMeals.Count >= maxResults)
                break;

            var meals = await SearchMealsByIngredientAsync(term);
            
            foreach (var meal in meals)
            {
                if (allMeals.Count >= maxResults)
                    break;
                    
                if (!seenIds.Contains(meal.IdMeal))
                {
                    seenIds.Add(meal.IdMeal);
                    allMeals.Add(meal);
                }
            }
        }

        return allMeals;
    }
}
