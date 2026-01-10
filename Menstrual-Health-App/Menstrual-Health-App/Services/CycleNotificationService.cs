using Menstrual_Health_App.Models;

namespace Menstrual_Health_App.Services;

/// <summary>
/// Service for scheduling cycle phase notifications
/// </summary>
public class CycleNotificationService
{
    private readonly INotificationManagerService? _notificationManager;
    private readonly CycleDataService _cycleDataService;

    public CycleNotificationService(INotificationManagerService? notificationManager, CycleDataService cycleDataService)
    {
        _notificationManager = notificationManager;
        _cycleDataService = cycleDataService;
    }

    /// <summary>
    /// Schedule notifications for the upcoming cycle phases
    /// </summary>
    public void SchedulePhaseNotifications()
    {
        if (_notificationManager == null)
            return;

        var cycleData = _cycleDataService.GetCycleData();
        if (!cycleData.NotificationsEnabled)
            return;

        // Cancel existing notifications first
        _notificationManager.CancelAllNotifications();

        var today = DateTime.Today;
        var notificationTime = cycleData.NotificationTime;

        // Schedule notifications for the next 2 cycles
        for (int cycle = 0; cycle < 2; cycle++)
        {
            var cycleStartDate = cycleData.LastPeriodStartDate.AddDays(cycle * cycleData.CycleLengthDays);
            
            // Menstrual phase notification (day 1)
            var menstrualDate = cycleStartDate;
            if (menstrualDate > today)
            {
                SchedulePhaseNotification(CyclePhase.Menstrual, menstrualDate, notificationTime);
            }

            // Follicular phase notification (after period ends)
            var follicularDate = cycleStartDate.AddDays(cycleData.PeriodLengthDays);
            if (follicularDate > today)
            {
                SchedulePhaseNotification(CyclePhase.Follicular, follicularDate, notificationTime);
            }

            // Ovulation phase notification (around day 14, or 14 days before next period)
            var ovulationDate = cycleStartDate.AddDays(cycleData.CycleLengthDays - 14 - 1);
            if (ovulationDate > today)
            {
                SchedulePhaseNotification(CyclePhase.Ovulation, ovulationDate, notificationTime);
            }

            // Luteal phase notification (after ovulation)
            var lutealDate = cycleStartDate.AddDays(cycleData.CycleLengthDays - 14 + 2);
            if (lutealDate > today)
            {
                SchedulePhaseNotification(CyclePhase.Luteal, lutealDate, notificationTime);
            }
        }

        // Schedule period reminder (3 days before expected period)
        var nextPeriod = cycleData.GetNextPeriodDate();
        var reminderDate = nextPeriod.AddDays(-3);
        if (reminderDate > today)
        {
            var reminderDateTime = reminderDate.Add(notificationTime);
            _notificationManager.SendNotification(
                "Period Coming Soon",
                $"Your period is expected in 3 days. Time to prepare!",
                reminderDateTime,
                CyclePhase.Menstrual
            );
        }
    }

    private void SchedulePhaseNotification(CyclePhase phase, DateTime date, TimeSpan time)
    {
        if (_notificationManager == null)
            return;

        var notificationDateTime = date.Add(time);
        var (title, message) = GetPhaseNotificationContent(phase);

        _notificationManager.SendNotification(title, message, notificationDateTime, phase);
    }

    private (string title, string message) GetPhaseNotificationContent(CyclePhase phase)
    {
        return phase switch
        {
            CyclePhase.Menstrual => (
                "?? Menstrual Phase Starting",
                "Your period is here. Tap for tips on staying comfortable and healthy."
            ),
            CyclePhase.Follicular => (
                "?? Follicular Phase Begins",
                "Energy rising! Tap for tips to make the most of this productive phase."
            ),
            CyclePhase.Ovulation => (
                "?? Ovulation Window",
                "You're at your peak! Tap to learn more about this fertile phase."
            ),
            CyclePhase.Luteal => (
                "?? Luteal Phase Starting",
                "Time to slow down. Tap for self-care tips and PMS management."
            ),
            _ => ("Cycle Update", "Tap to check your current phase")
        };
    }

    /// <summary>
    /// Send an immediate notification for the current phase
    /// </summary>
    public void SendCurrentPhaseNotification()
    {
        if (_notificationManager == null)
            return;

        var cycleData = _cycleDataService.GetCycleData();
        var currentPhase = cycleData.GetCurrentPhase();
        var (title, message) = GetPhaseNotificationContent(currentPhase);

        _notificationManager.SendNotification(title, message, null, currentPhase);
    }
}
