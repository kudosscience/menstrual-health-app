namespace Menstrual_Health_App.Models;

/// <summary>
/// Event arguments for notification received events
/// </summary>
public class NotificationEventArgs : EventArgs
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public CyclePhase? Phase { get; set; }
}
