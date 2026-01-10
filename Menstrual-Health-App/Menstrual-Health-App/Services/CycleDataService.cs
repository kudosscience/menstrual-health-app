using System.Text.Json;
using Menstrual_Health_App.Models;

namespace Menstrual_Health_App.Services;

/// <summary>
/// Service for persisting cycle data using Preferences
/// </summary>
public class CycleDataService
{
    private const string CycleDataKey = "cycle_data";
    private const string PeriodLogsKey = "period_logs";
    private const string HasCompletedSetupKey = "has_completed_setup";

    private CycleData? _cachedCycleData;
    private List<PeriodLog>? _cachedPeriodLogs;

    public CycleData GetCycleData()
    {
        if (_cachedCycleData != null)
            return _cachedCycleData;

        var json = Preferences.Get(CycleDataKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            _cachedCycleData = new CycleData
            {
                LastPeriodStartDate = DateTime.Today.AddDays(-14),
                CycleLengthDays = 28,
                PeriodLengthDays = 5
            };
            return _cachedCycleData;
        }

        try
        {
            _cachedCycleData = JsonSerializer.Deserialize<CycleData>(json) ?? new CycleData();
        }
        catch
        {
            _cachedCycleData = new CycleData();
        }

        return _cachedCycleData;
    }

    public void SaveCycleData(CycleData data)
    {
        _cachedCycleData = data;
        var json = JsonSerializer.Serialize(data);
        Preferences.Set(CycleDataKey, json);
    }

    public List<PeriodLog> GetPeriodLogs()
    {
        if (_cachedPeriodLogs != null)
            return _cachedPeriodLogs;

        var json = Preferences.Get(PeriodLogsKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            _cachedPeriodLogs = new List<PeriodLog>();
            return _cachedPeriodLogs;
        }

        try
        {
            _cachedPeriodLogs = JsonSerializer.Deserialize<List<PeriodLog>>(json) ?? new List<PeriodLog>();
        }
        catch
        {
            _cachedPeriodLogs = new List<PeriodLog>();
        }

        return _cachedPeriodLogs;
    }

    public void SavePeriodLogs(List<PeriodLog> logs)
    {
        _cachedPeriodLogs = logs;
        var json = JsonSerializer.Serialize(logs);
        Preferences.Set(PeriodLogsKey, json);
    }

    public void LogPeriodStart(DateTime startDate)
    {
        var logs = GetPeriodLogs();
        
        // Check if there's an ongoing period without end date
        var ongoingPeriod = logs.FirstOrDefault(l => !l.EndDate.HasValue);
        if (ongoingPeriod != null)
        {
            ongoingPeriod.EndDate = startDate.AddDays(-1);
        }

        logs.Add(new PeriodLog { StartDate = startDate });
        SavePeriodLogs(logs);

        // Update cycle data
        var cycleData = GetCycleData();
        cycleData.LastPeriodStartDate = startDate;
        
        // Calculate average cycle length from logs
        if (logs.Count >= 2)
        {
            var recentLogs = logs.OrderByDescending(l => l.StartDate).Take(6).ToList();
            if (recentLogs.Count >= 2)
            {
                var cycleLengths = new List<int>();
                for (int i = 0; i < recentLogs.Count - 1; i++)
                {
                    var length = (recentLogs[i].StartDate - recentLogs[i + 1].StartDate).Days;
                    if (length > 20 && length < 40) // Valid cycle length
                        cycleLengths.Add(length);
                }
                if (cycleLengths.Count > 0)
                    cycleData.CycleLengthDays = (int)Math.Round(cycleLengths.Average());
            }
        }

        SaveCycleData(cycleData);
    }

    public void LogPeriodEnd(DateTime endDate)
    {
        var logs = GetPeriodLogs();
        var ongoingPeriod = logs.FirstOrDefault(l => !l.EndDate.HasValue);
        
        if (ongoingPeriod != null)
        {
            ongoingPeriod.EndDate = endDate;
            SavePeriodLogs(logs);

            // Update average period length
            var cycleData = GetCycleData();
            var completedLogs = logs.Where(l => l.EndDate.HasValue).Take(6).ToList();
            if (completedLogs.Count > 0)
            {
                cycleData.PeriodLengthDays = (int)Math.Round(completedLogs.Average(l => l.DurationDays));
                SaveCycleData(cycleData);
            }
        }
    }

    public bool HasCompletedSetup()
    {
        return Preferences.Get(HasCompletedSetupKey, false);
    }

    public void SetSetupCompleted()
    {
        Preferences.Set(HasCompletedSetupKey, true);
    }

    public void ClearAllData()
    {
        _cachedCycleData = null;
        _cachedPeriodLogs = null;
        Preferences.Remove(CycleDataKey);
        Preferences.Remove(PeriodLogsKey);
        Preferences.Remove(HasCompletedSetupKey);
    }
}
