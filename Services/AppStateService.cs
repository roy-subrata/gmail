namespace GmailApp.Services;

public class AppStateService
{
    public event Action? OnLogsChanged;
    public event Action<int, int>? OnLogToggled;

    public void NotifyLogsChanged() => OnLogsChanged?.Invoke();
    /// <param name="pageMode">0=Loading 1=Code 2=GmailApp 3=Mail 4=Error</param>
    public void NotifyLogToggled(int logId, int pageMode)
        => OnLogToggled?.Invoke(logId, pageMode);
}
