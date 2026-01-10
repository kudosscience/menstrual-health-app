using Menstrual_Health_App.Models;

namespace Menstrual_Health_App.Services;

/// <summary>
/// Interface for notification management across platforms
/// </summary>
public interface INotificationManagerService
{
    event EventHandler<NotificationEventArgs>? NotificationReceived;
    
    void SendNotification(string title, string message, DateTime? notifyTime = null, CyclePhase? phase = null);
    void ReceiveNotification(string title, string message, CyclePhase? phase = null);
    void CancelAllNotifications();
}
