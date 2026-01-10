namespace Menstrual_Health_App.Models;

/// <summary>
/// Represents the different phases of the menstrual cycle
/// </summary>
public enum CyclePhase
{
    Menstrual,      // Days 1-5: Period/menstruation
    Follicular,     // Days 6-14: Follicular phase (after period, before ovulation)
    Ovulation,      // Days 14-16: Ovulation window
    Luteal          // Days 17-28: Luteal phase (after ovulation, before period)
}
