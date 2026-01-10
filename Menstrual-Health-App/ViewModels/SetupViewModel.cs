using System.Windows.Input;
using Menstrual_Health_App.Services;

namespace Menstrual_Health_App.ViewModels;

public class SetupViewModel : BaseViewModel
{
    private readonly CycleDataService _cycleDataService;
    private readonly CycleNotificationService _cycleNotificationService;

    private DateTime _lastPeriodDate = DateTime.Today.AddDays(-14);
    private int _cycleLengthDays = 28;
    private int _periodLengthDays = 5;
    private bool _notificationsEnabled = true;

    public DateTime LastPeriodDate
    {
        get => _lastPeriodDate;
        set => SetProperty(ref _lastPeriodDate, value);
    }

    public int CycleLengthDays
    {
        get => _cycleLengthDays;
        set => SetProperty(ref _cycleLengthDays, value);
    }

    public int PeriodLengthDays
    {
        get => _periodLengthDays;
        set => SetProperty(ref _periodLengthDays, value);
    }

    public bool NotificationsEnabled
    {
        get => _notificationsEnabled;
        set => SetProperty(ref _notificationsEnabled, value);
    }

    public DateTime MinimumDate => DateTime.Today.AddYears(-1);
    public DateTime MaximumDate => DateTime.Today;

    public ICommand CompleteSetupCommand { get; }

    public SetupViewModel(CycleDataService cycleDataService, CycleNotificationService cycleNotificationService)
    {
        _cycleDataService = cycleDataService;
        _cycleNotificationService = cycleNotificationService;

        CompleteSetupCommand = new Command(async () => await CompleteSetupAsync());
    }

    private async Task CompleteSetupAsync()
    {
        var cycleData = new Models.CycleData
        {
            LastPeriodStartDate = LastPeriodDate,
            CycleLengthDays = CycleLengthDays,
            PeriodLengthDays = PeriodLengthDays,
            NotificationsEnabled = NotificationsEnabled,
            NotificationTime = new TimeSpan(9, 0, 0)
        };

        _cycleDataService.SaveCycleData(cycleData);
        _cycleDataService.SetSetupCompleted();

        if (NotificationsEnabled)
        {
            // Request notification permissions on Android
#if ANDROID
            var status = await Permissions.RequestAsync<Menstrual_Health_App.Platforms.Android.NotificationPermission>();
#endif
            _cycleNotificationService.SchedulePhaseNotifications();
        }

        await Shell.Current.GoToAsync("//MainPage");
    }
}
