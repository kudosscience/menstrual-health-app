using Menstrual_Health_App.Views;

namespace Menstrual_Health_App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute(nameof(PhaseDetailPage), typeof(PhaseDetailPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(SetupPage), typeof(SetupPage));
        }
    }
}
