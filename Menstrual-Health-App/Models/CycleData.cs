namespace Menstrual_Health_App.Models;

/// <summary>
/// Represents user's menstrual cycle data
/// </summary>
public class CycleData
{
    /// <summary>
    /// The start date of the last period
    /// </summary>
    public DateTime LastPeriodStartDate { get; set; }

    /// <summary>
    /// Average cycle length in days (default is 28 days)
    /// </summary>
    public int CycleLengthDays { get; set; } = 28;

    /// <summary>
    /// Average period length in days (default is 5 days)
    /// </summary>
    public int PeriodLengthDays { get; set; } = 5;

    /// <summary>
    /// Whether notifications are enabled
    /// </summary>
    public bool NotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Time of day to send notifications
    /// </summary>
    public TimeSpan NotificationTime { get; set; } = new TimeSpan(9, 0, 0); // 9:00 AM

    /// <summary>
    /// Calculate the current day of the cycle
    /// </summary>
    public int GetCurrentCycleDay()
    {
        var daysSinceStart = (DateTime.Today - LastPeriodStartDate.Date).Days;
        return (daysSinceStart % CycleLengthDays) + 1;
    }

    /// <summary>
    /// Calculate the current phase of the cycle
    /// </summary>
    public CyclePhase GetCurrentPhase()
    {
        var cycleDay = GetCurrentCycleDay();
        return GetPhaseForDay(cycleDay);
    }

    /// <summary>
    /// Get the phase for a specific day of the cycle
    /// </summary>
    public CyclePhase GetPhaseForDay(int cycleDay)
    {
        if (cycleDay <= PeriodLengthDays)
            return CyclePhase.Menstrual;

        int ovulationDay = CycleLengthDays - 14; // Ovulation typically occurs 14 days before next period
        int ovulationStart = ovulationDay - 1;
        int ovulationEnd = ovulationDay + 2;

        if (cycleDay > PeriodLengthDays && cycleDay < ovulationStart)
            return CyclePhase.Follicular;

        if (cycleDay >= ovulationStart && cycleDay <= ovulationEnd)
            return CyclePhase.Ovulation;

        return CyclePhase.Luteal;
    }

    /// <summary>
    /// Calculate the expected next period start date
    /// </summary>
    public DateTime GetNextPeriodDate()
    {
        var daysSinceStart = (DateTime.Today - LastPeriodStartDate.Date).Days;
        var cyclesCompleted = daysSinceStart / CycleLengthDays;
        return LastPeriodStartDate.AddDays((cyclesCompleted + 1) * CycleLengthDays);
    }

    /// <summary>
    /// Get days until next period
    /// </summary>
    public int GetDaysUntilNextPeriod()
    {
        return (GetNextPeriodDate() - DateTime.Today).Days;
    }

    /// <summary>
    /// Calculate the expected ovulation date
    /// </summary>
    public DateTime GetNextOvulationDate()
    {
        var nextPeriod = GetNextPeriodDate();
        return nextPeriod.AddDays(-14);
    }
}
