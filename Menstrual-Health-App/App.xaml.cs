using Menstrual_Health_App.Services;

namespace Menstrual_Health_App
{
    public partial class App : Application
    {
        private readonly CycleDataService _cycleDataService;

        public App(CycleDataService cycleDataService)
        {
            InitializeComponent();
            _cycleDataService = cycleDataService;
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Check if user has completed setup
            if (!_cycleDataService.HasCompletedSetup())
            {
                await Shell.Current.GoToAsync("//MainPage/SetupPage");
            }
        }
    }
}
