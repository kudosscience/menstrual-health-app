using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Menstrual_Health_App.Platforms.Android;
using Menstrual_Health_App.Services;

namespace Menstrual_Health_App
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CreateNotificationFromIntent(Intent);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            CreateNotificationFromIntent(intent);
        }

        static void CreateNotificationFromIntent(Intent? intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(NotificationManagerService.TitleKey) ?? "";
                string message = intent.GetStringExtra(NotificationManagerService.MessageKey) ?? "";
                int phaseInt = intent.GetIntExtra(NotificationManagerService.PhaseKey, -1);
                
                var phase = phaseInt >= 0 ? (Menstrual_Health_App.Models.CyclePhase?)phaseInt : null;

                if (!string.IsNullOrEmpty(title))
                {
                    var service = IPlatformApplication.Current?.Services.GetService<INotificationManagerService>();
                    service?.ReceiveNotification(title, message, phase);
                }
            }
        }
    }
}
