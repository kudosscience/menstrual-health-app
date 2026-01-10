using Android.Content;

namespace Menstrual_Health_App.Platforms.Android;

[BroadcastReceiver(Enabled = true, Label = "Cycle Notifications Broadcast Receiver")]
public class AlarmHandler : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent?.Extras != null)
        {
            string title = intent.GetStringExtra(NotificationManagerService.TitleKey) ?? "Cycle Update";
            string message = intent.GetStringExtra(NotificationManagerService.MessageKey) ?? "";
            int phaseInt = intent.GetIntExtra(NotificationManagerService.PhaseKey, -1);
            
            var phase = phaseInt >= 0 ? (Menstrual_Health_App.Models.CyclePhase?)phaseInt : null;

            NotificationManagerService manager = NotificationManagerService.Instance ?? new NotificationManagerService();
            manager.Show(title, message, phase);
        }
    }
}
