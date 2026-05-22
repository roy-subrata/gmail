using GmailApp.Data;
using GmailApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GmailApp.Services;

public class CampaignLinkService(AppDbContext db)
{
    public async Task<CampaignLink> CreateAsync(string domain, string slug)
    {
        var link = new CampaignLink
        {
            Domain = domain.Trim(),
            Slug = slug.Trim().TrimStart('/'),
            CreatedAt = DateTime.UtcNow
        };
        db.CampaignLinks.Add(link);
        await db.SaveChangesAsync();
        return link;
    }

    public Task<List<CampaignLink>> GetAllAsync()
        => db.CampaignLinks
             .AsNoTracking()
             .Include(c => c.LoginLogs)
             .OrderByDescending(c => c.CreatedAt)
             .ToListAsync();

    public async Task DeleteAsync(int id)
    {
        var link = await db.CampaignLinks.FindAsync(id);
        if (link is null) return;
        db.CampaignLinks.Remove(link);
        await db.SaveChangesAsync();
    }

    public Task<CampaignLink?> GetBySlugAsync(string slug)
        => db.CampaignLinks.FirstOrDefaultAsync(c => c.Slug == slug);

    public async Task LockAsync(int id)
    {
        var link = await db.CampaignLinks.FindAsync(id);
        if (link is null) return;
        link.IsLocked = true;
        link.PageMode  = 0;   // default: Loading — admin decides next
        await db.SaveChangesAsync();
    }

    public async Task ResetAllLocksAsync()
    {
        var links = await db.CampaignLinks.ToListAsync();
        foreach (var l in links)
        {
            l.IsLocked = false;
            l.PageMode  = 0;
        }
        await db.SaveChangesAsync();
    }

    public async Task SetPageModeAsync(int id, int pageMode, AppStateService appState)
    {
        var link = await db.CampaignLinks.FindAsync(id);
        if (link is null) return;
        link.PageMode = pageMode;
        await db.SaveChangesAsync();
        appState.NotifyCampaignToggled(id, pageMode);
    }
}
