using Microsoft.Extensions.Logging;
using Menstrual_Health_App.Services;
using Menstrual_Health_App.ViewModels;
using Menstrual_Health_App.Views;

namespace Menstrual_Health_App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Services
            builder.Services.AddSingleton<CycleDataService>();
            
            // Register platform-specific notification service
#if ANDROID
            builder.Services.AddSingleton<INotificationManagerService, Menstrual_Health_App.Platforms.Android.NotificationManagerService>();
#elif IOS
            builder.Services.AddSingleton<INotificationManagerService, Menstrual_Health_App.Platforms.iOS.NotificationManagerService>();
#elif MACCATALYST
            builder.Services.AddSingleton<INotificationManagerService, Menstrual_Health_App.Platforms.MacCatalyst.NotificationManagerService>();
#elif WINDOWS
            builder.Services.AddSingleton<INotificationManagerService, Menstrual_Health_App.Platforms.Windows.NotificationManagerService>();
#endif

            builder.Services.AddSingleton<CycleNotificationService>();

            // Register ViewModels
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<PhaseDetailViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SetupViewModel>();

            // Register Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<PhaseDetailPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<SetupPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
