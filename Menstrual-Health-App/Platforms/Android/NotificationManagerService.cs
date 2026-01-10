using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Menstrual_Health_App.Models;
using Menstrual_Health_App.Services;

namespace Menstrual_Health_App.Platforms.Android;

public class NotificationManagerService : INotificationManagerService
{
    const string channelId = "menstrual_health_channel";
    const string channelName = "Cycle Notifications";
    const string channelDescription = "Notifications for menstrual cycle phases and reminders.";

    public const string TitleKey = "title";
    public const string MessageKey = "message";
    public const string PhaseKey = "phase";

    bool channelInitialized = false;
    int messageId = 0;
    int pendingIntentId = 0;

    NotificationManagerCompat? compatManager;

    public event EventHandler<NotificationEventArgs>? NotificationReceived;

    public static NotificationManagerService? Instance { get; private set; }

    public NotificationManagerService()
    {
        if (Instance == null)
        {
            CreateNotificationChannel();
            compatManager = NotificationManagerCompat.From(Platform.AppContext);
            Instance = this;
        }
    }

    public void SendNotification(string title, string message, DateTime? notifyTime = null, CyclePhase? phase = null)
    {
        if (!channelInitialized)
        {
            CreateNotificationChannel();
        }

        if (notifyTime != null)
        {
            Intent intent = new Intent(Platform.AppContext, typeof(AlarmHandler));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);
            if (phase.HasValue)
                intent.PutExtra(PhaseKey, (int)phase.Value);
            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

            var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.CancelCurrent;

            PendingIntent? pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
            long triggerTime = GetNotifyTime(notifyTime.Value);
            AlarmManager? alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager?.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }
        else
        {
            Show(title, message, phase);
        }
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
        var notificationManager = (NotificationManager?)Platform.AppContext.GetSystemService(Context.NotificationService);
        notificationManager?.CancelAll();
    }

    public void Show(string title, string message, CyclePhase? phase = null)
    {
        Intent intent = new Intent(Platform.AppContext, typeof(MainActivity));
        intent.PutExtra(TitleKey, title);
        intent.PutExtra(MessageKey, message);
        if (phase.HasValue)
            intent.PutExtra(PhaseKey, (int)phase.Value);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.UpdateCurrent;

        PendingIntent? pendingIntent = PendingIntent.GetActivity(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
        
        NotificationCompat.Builder builder = new NotificationCompat.Builder(Platform.AppContext, channelId)
            .SetContentIntent(pendingIntent)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetSmallIcon(Resource.Drawable.notification_icon)
            .SetAutoCancel(true)
            .SetPriority(NotificationCompat.PriorityDefault);

        Notification? notification = builder.Build();
        if (notification != null)
            compatManager?.Notify(messageId++, notification);
    }

    void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channelNameJava = new Java.Lang.String(channelName);
            var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
            {
                Description = channelDescription
            };
            NotificationManager? manager = (NotificationManager?)Platform.AppContext.GetSystemService(Context.NotificationService);
            manager?.CreateNotificationChannel(channel);
            channelInitialized = true;
        }
    }

    long GetNotifyTime(DateTime notifyTime)
    {
        DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
        double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
        long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
        return utcAlarmTime;
    }
}
