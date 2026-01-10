namespace Menstrual_Health_App.Models;

/// <summary>
/// Represents a logged period entry
/// </summary>
public class PeriodLog
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int FlowIntensity { get; set; } = 2; // 1-5 scale
    public List<string> Symptoms { get; set; } = new();
    public string Notes { get; set; } = string.Empty;

    public int DurationDays => EndDate.HasValue 
        ? (EndDate.Value - StartDate).Days + 1 
        : (DateTime.Today - StartDate).Days + 1;
}
