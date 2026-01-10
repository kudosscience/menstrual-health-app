using Menstrual_Health_App.Models;
using Menstrual_Health_App.Services;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace Menstrual_Health_App.Platforms.Windows;

public class NotificationManagerService : INotificationManagerService
{
    private bool isInitialized;
    private int messageId;

    public event EventHandler<NotificationEventArgs>? NotificationReceived;

    public NotificationManagerService()
    {
        // Defer initialization until actually needed - calling Register() during
        // DI container build causes COMException because the app isn't fully initialized yet
    }

    private bool initializationFailed;

    private void Initialize()
    {
        if (isInitialized || initializationFailed)
            return;

        try
        {
            var notificationManager = AppNotificationManager.Default;
            notificationManager.NotificationInvoked += OnNotificationInvoked;
            notificationManager.Register();
            isInitialized = true;
        }
        catch (Exception ex)
        {
            // AppNotificationManager.Register() can throw COMException if app identity
            // is not properly configured or notifications are not available.
            // Mark as failed to avoid repeated attempts and allow the app to continue.
            initializationFailed = true;
            System.Diagnostics.Debug.WriteLine($"Failed to initialize Windows notifications: {ex.Message}");
        }
    }

    private void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        var arguments = args.Arguments;
        
        string title = "";
        string message = "";
        CyclePhase? phase = null;

        if (args.Arguments.TryGetValue("title", out var titleValue))
            title = titleValue;
        if (args.Arguments.TryGetValue("message", out var messageValue))
            message = messageValue;
        if (args.Arguments.TryGetValue("phase", out var phaseValue) && int.TryParse(phaseValue, out var phaseInt))
            phase = (CyclePhase)phaseInt;

        ReceiveNotification(title, message, phase);
    }

    public void SendNotification(string title, string message, DateTime? notifyTime = null, CyclePhase? phase = null)
    {
        if (!isInitialized)
            Initialize();

        if (!isInitialized)
            return; // Initialization failed, skip notification

        messageId++;

        var builder = new AppNotificationBuilder()
            .AddArgument("title", title)
            .AddArgument("message", message)
            .AddText(title)
            .AddText(message);

        if (phase.HasValue)
        {
            builder.AddArgument("phase", ((int)phase.Value).ToString());
        }

        var notification = builder.BuildNotification();

        // Note: Windows App SDK doesn't support scheduled notifications yet
        // Notifications are sent immediately
        AppNotificationManager.Default.Show(notification);
    }

    public void ReceiveNotification(string title, string message, CyclePhase? phase = null)
    {
        var args = new NotificationEventArgs
        {
            Title = title,
            Message = message,
            Phase = phase
        };
        NotificationReceived?.Invoke(null, args);
    }

    public void CancelAllNotifications()
    {
        if (!isInitialized)
            Initialize();

        if (!isInitialized)
            return; // Initialization failed, skip cancellation

        // Remove all notifications from notification center
        AppNotificationManager.Default.RemoveAllAsync().AsTask().Wait();
    }
}
