using Foundation;
using UserNotifications;
using Menstrual_Health_App.Models;
using Menstrual_Health_App.Services;

namespace Menstrual_Health_App.Platforms.iOS;

public class NotificationManagerService : INotificationManagerService
{
    int messageId = 0;
    bool hasNotificationsPermission;

    public event EventHandler<NotificationEventArgs>? NotificationReceived;

    public NotificationManagerService()
    {
        UNUserNotificationCenter.Current.Delegate = new NotificationReceiver();

        UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (approved, err) =>
        {
            hasNotificationsPermission = approved;
        });
    }

    public void SendNotification(string title, string message, DateTime? notifyTime = null, CyclePhase? phase = null)
    {
        if (!hasNotificationsPermission)
            return;

        messageId++;
        var content = new UNMutableNotificationContent()
        {
            Title = title,
            Subtitle = "",
            Body = message,
            Badge = 1
        };

        if (phase.HasValue)
        {
            content.UserInfo = NSDictionary.FromObjectAndKey(
                new NSNumber((int)phase.Value), 
                new NSString("phase")
            );
        }

        UNNotificationTrigger trigger;
        if (notifyTime != null)
        {
            trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNSDateComponents(notifyTime.Value), false);
        }
        else
        {
            trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.25, false);
        }

        var request = UNNotificationRequest.FromIdentifier(messageId.ToString(), content, trigger);
        UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
        {
            if (err != null)
                System.Diagnostics.Debug.WriteLine($"Failed to schedule notification: {err}");
        });
    }

    public void ReceiveNotification(string title, string message, CyclePhase? phase = null)
    {
        var args = new NotificationEventArgs()
        {
            Title = title,
            Message = message,
            Phase = phase
        };
        NotificationReceived?.Invoke(null, args);
    }

    public void CancelAllNotifications()
    {
        UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
    }

    NSDateComponents GetNSDateComponents(DateTime dateTime)
    {
        return new NSDateComponents
        {
            Month = dateTime.Month,
            Day = dateTime.Day,
            Year = dateTime.Year,
            Hour = dateTime.Hour,
            Minute = dateTime.Minute,
            Second = dateTime.Second
        };
    }
}
