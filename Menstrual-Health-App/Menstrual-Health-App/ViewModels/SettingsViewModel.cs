using System.Windows.Input;
using Menstrual_Health_App.Models;
using Menstrual_Health_App.Services;

namespace Menstrual_Health_App.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly CycleDataService _cycleDataService;
    private readonly CycleNotificationService _cycleNotificationService;

    private CycleData _cycleData = new();
    private DateTime _lastPeriodDate;
    private int _cycleLengthDays;
    private int _periodLengthDays;
    private bool _notificationsEnabled;
    private TimeSpan _notificationTime;

    public DateTime LastPeriodDate
    {
        get => _lastPeriodDate;
        set
        {
            if (SetProperty(ref _lastPeriodDate, value))
            {
                _cycleData.LastPeriodStartDate = value;
            }
        }
    }

    public int CycleLengthDays
    {
        get => _cycleLengthDays;
        set
        {
            if (SetProperty(ref _cycleLengthDays, value))
            {
                _cycleData.CycleLengthDays = value;
            }
        }
    }

    public int PeriodLengthDays
    {
        get => _periodLengthDays;
        set
        {
            if (SetProperty(ref _periodLengthDays, value))
            {
                _cycleData.PeriodLengthDays = value;
            }
        }
    }

    public bool NotificationsEnabled
    {
        get => _notificationsEnabled;
        set
        {
            if (SetProperty(ref _notificationsEnabled, value))
            {
                _cycleData.NotificationsEnabled = value;
            }
        }
    }

    public TimeSpan NotificationTime
    {
        get => _notificationTime;
        set
        {
            if (SetProperty(ref _notificationTime, value))
            {
                _cycleData.NotificationTime = value;
            }
        }
    }

    public DateTime MinimumDate => DateTime.Today.AddYears(-1);
    public DateTime MaximumDate => DateTime.Today;

    public ICommand SaveCommand { get; }
    public ICommand ResetDataCommand { get; }
    public ICommand TestNotificationCommand { get; }

    public SettingsViewModel(CycleDataService cycleDataService, CycleNotificationService cycleNotificationService)
    {
        _cycleDataService = cycleDataService;
        _cycleNotificationService = cycleNotificationService;

        SaveCommand = new Command(async () => await SaveSettingsAsync());
        ResetDataCommand = new Command(async () => await ResetDataAsync());
        TestNotificationCommand = new Command(() => TestNotification());

        LoadSettings();
    }

    private void LoadSettings()
    {
        _cycleData = _cycleDataService.GetCycleData();
        LastPeriodDate = _cycleData.LastPeriodStartDate;
        CycleLengthDays = _cycleData.CycleLengthDays;
        PeriodLengthDays = _cycleData.PeriodLengthDays;
        NotificationsEnabled = _cycleData.NotificationsEnabled;
        NotificationTime = _cycleData.NotificationTime;
    }

    private async Task SaveSettingsAsync()
    {
        _cycleDataService.SaveCycleData(_cycleData);
        _cycleNotificationService.SchedulePhaseNotifications();

        await Application.Current!.MainPage!.DisplayAlert(
            "Settings Saved",
            "Your settings have been saved and notifications have been updated.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async Task ResetDataAsync()
    {
        var result = await Application.Current!.MainPage!.DisplayAlert(
            "Reset All Data",
            "This will delete all your cycle data and settings. This action cannot be undone. Are you sure?",
            "Reset",
            "Cancel");

        if (result)
        {
            _cycleDataService.ClearAllData();
            LoadSettings();

            await Application.Current.MainPage.DisplayAlert(
                "Data Reset",
                "All your data has been reset.",
                "OK");
        }
    }

    private void TestNotification()
    {
        _cycleNotificationService.SendCurrentPhaseNotification();
    }
}
