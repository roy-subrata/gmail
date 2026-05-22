namespace GmailApp.Services;

public class AppStateService
{
    public event Action? OnLogsChanged;
    public event Action<int, int>? OnCampaignToggled;

    public void NotifyLogsChanged() => OnLogsChanged?.Invoke();
    /// <param name="pageMode">0=Loading 1=Code 2=GmailApp</param>
    public void NotifyCampaignToggled(int campaignId, int pageMode)
        => OnCampaignToggled?.Invoke(campaignId, pageMode);
}
