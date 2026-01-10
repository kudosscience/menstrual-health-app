using UserNotifications;
using Menstrual_Health_App.Services;

namespace Menstrual_Health_App.Platforms.MacCatalyst;

public class NotificationReceiver : UNUserNotificationCenterDelegate
{
    public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        ProcessNotification(notification);

        var presentationOptions = (OperatingSystem.IsMacCatalystVersionAtLeast(14))
            ? UNNotificationPresentationOptions.Banner
            : UNNotificationPresentationOptions.Alert;

        completionHandler(presentationOptions);
    }

    public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
    {
        if (response.IsDefaultAction)
            ProcessNotification(response.Notification);

        completionHandler();
    }

    void ProcessNotification(UNNotification notification)
    {
        string title = notification.Request.Content.Title;
        string message = notification.Request.Content.Body;
        
        Menstrual_Health_App.Models.CyclePhase? phase = null;
        var userInfo = notification.Request.Content.UserInfo;
        if (userInfo != null && userInfo.TryGetValue(new Foundation.NSString("phase"), out var phaseValue))
        {
            if (phaseValue is Foundation.NSNumber phaseNumber)
            {
                phase = (Menstrual_Health_App.Models.CyclePhase)phaseNumber.Int32Value;
            }
        }

        var service = IPlatformApplication.Current?.Services.GetService<INotificationManagerService>();
        service?.ReceiveNotification(title, message, phase);
    }
}
