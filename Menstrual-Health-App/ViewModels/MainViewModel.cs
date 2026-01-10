using System.Windows.Input;
using Menstrual_Health_App.Models;
using Menstrual_Health_App.Services;
using Menstrual_Health_App.Views;

namespace Menstrual_Health_App.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly CycleDataService _cycleDataService;
    private readonly CycleNotificationService _cycleNotificationService;
    private readonly INotificationManagerService? _notificationManager;

    private CycleData _cycleData = new();
    private PhaseInfo _currentPhaseInfo = new();
    private int _currentCycleDay;
    private int _daysUntilNextPeriod;
    private DateTime _nextPeriodDate;
    private DateTime _nextOvulationDate;
    private bool _isLoading;

    public CycleData CycleData
    {
        get => _cycleData;
        set => SetProperty(ref _cycleData, value);
    }

    public PhaseInfo CurrentPhaseInfo
    {
        get => _currentPhaseInfo;
        set => SetProperty(ref _currentPhaseInfo, value);
    }

    public int CurrentCycleDay
    {
        get => _currentCycleDay;
        set => SetProperty(ref _currentCycleDay, value);
    }

    public int DaysUntilNextPeriod
    {
        get => _daysUntilNextPeriod;
        set => SetProperty(ref _daysUntilNextPeriod, value);
    }

    public DateTime NextPeriodDate
    {
        get => _nextPeriodDate;
        set => SetProperty(ref _nextPeriodDate, value);
    }

    public DateTime NextOvulationDate
    {
        get => _nextOvulationDate;
        set => SetProperty(ref _nextOvulationDate, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string CycleDayDisplay => $"Day {CurrentCycleDay}";
    public string DaysUntilPeriodDisplay => DaysUntilNextPeriod == 1 ? "1 day" : $"{DaysUntilNextPeriod} days";
    public string NextPeriodDisplay => NextPeriodDate.ToString("MMM d");
    public string NextOvulationDisplay => NextOvulationDate.ToString("MMM d");

    public ICommand RefreshCommand { get; }
    public ICommand LogPeriodStartCommand { get; }
    public ICommand LogPeriodEndCommand { get; }
    public ICommand ViewPhaseInfoCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public ICommand ViewPhaseCommand { get; }

    public MainViewModel(CycleDataService cycleDataService, CycleNotificationService cycleNotificationService, INotificationManagerService? notificationManager)
    {
        _cycleDataService = cycleDataService;
        _cycleNotificationService = cycleNotificationService;
        _notificationManager = notificationManager;

        RefreshCommand = new Command(async () => await RefreshDataAsync());
        LogPeriodStartCommand = new Command(async () => await LogPeriodStartAsync());
        LogPeriodEndCommand = new Command(async () => await LogPeriodEndAsync());
        ViewPhaseInfoCommand = new Command(async () => await ViewPhaseInfoAsync());
        OpenSettingsCommand = new Command(async () => await OpenSettingsAsync());
        ViewPhaseCommand = new Command<string>(async (phaseName) => await ViewPhaseByNameAsync(phaseName));

        // Subscribe to notification events
        if (_notificationManager != null)
        {
            _notificationManager.NotificationReceived += OnNotificationReceived;
        }

        _ = RefreshDataAsync();
    }

    private async void OnNotificationReceived(object? sender, NotificationEventArgs e)
    {
        if (e.Phase.HasValue)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await ViewPhaseAsync(e.Phase.Value);
            });
        }
    }

    public async Task RefreshDataAsync()
    {
        IsLoading = true;

        try
        {
            CycleData = _cycleDataService.GetCycleData();
            CurrentCycleDay = CycleData.GetCurrentCycleDay();
            DaysUntilNextPeriod = CycleData.GetDaysUntilNextPeriod();
            NextPeriodDate = CycleData.GetNextPeriodDate();
            NextOvulationDate = CycleData.GetNextOvulationDate();
            CurrentPhaseInfo = PhaseInfo.GetPhaseInfo(CycleData.GetCurrentPhase());

            OnPropertyChanged(nameof(CycleDayDisplay));
            OnPropertyChanged(nameof(DaysUntilPeriodDisplay));
            OnPropertyChanged(nameof(NextPeriodDisplay));
            OnPropertyChanged(nameof(NextOvulationDisplay));
        }
        finally
        {
            IsLoading = false;
        }

        await Task.CompletedTask;
    }

    private async Task LogPeriodStartAsync()
    {
        var result = await Application.Current!.MainPage!.DisplayAlert(
            "Log Period",
            "Did your period start today?",
            "Yes, Today",
            "Cancel");

        if (result)
        {
            _cycleDataService.LogPeriodStart(DateTime.Today);
            await RefreshDataAsync();
            _cycleNotificationService.SchedulePhaseNotifications();

            await Application.Current.MainPage.DisplayAlert(
                "Period Logged",
                "Your period has been logged. Your cycle predictions have been updated.",
                "OK");
        }
    }

    private async Task LogPeriodEndAsync()
    {
        var result = await Application.Current!.MainPage!.DisplayAlert(
            "End Period",
            "Did your period end today?",
            "Yes, Today",
            "Cancel");

        if (result)
        {
            _cycleDataService.LogPeriodEnd(DateTime.Today);
            await RefreshDataAsync();

            await Application.Current.MainPage.DisplayAlert(
                "Period Ended",
                "Your period end date has been logged.",
                "OK");
        }
    }

    private async Task ViewPhaseInfoAsync()
    {
        await ViewPhaseAsync(CycleData.GetCurrentPhase());
    }

    private async Task ViewPhaseByNameAsync(string? phaseName)
    {
        if (string.IsNullOrEmpty(phaseName))
            return;

        if (Enum.TryParse<CyclePhase>(phaseName, out var phase))
        {
            await ViewPhaseAsync(phase);
        }
    }

    private async Task ViewPhaseAsync(CyclePhase phase)
    {
        var phaseInfo = PhaseInfo.GetPhaseInfo(phase);
        await Shell.Current.GoToAsync($"{nameof(PhaseDetailPage)}", new Dictionary<string, object>
        {
            { "PhaseInfo", phaseInfo }
        });
    }

    private async Task OpenSettingsAsync()
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}
