namespace GmailApp.Models;

public class SmtpSettings
{
    public const string Section = "Smtp";

    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string ReplyTo { get; set; } = string.Empty;
    public bool UseStartTls { get; set; } = true;
}
