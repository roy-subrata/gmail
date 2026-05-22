using GmailApp.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GmailApp.Services;

public class EmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IOptions<SmtpSettings> options)
    {
        _smtp = options.Value;
    }

    public string SenderEmail => _smtp.SenderEmail;
    public string SenderName => _smtp.SenderName;

    public async Task<(bool Success, string Error)> SendAsync(
        string from, string to, string subject, string htmlBody)
    {
        try
        {
            var senderEmail = string.IsNullOrWhiteSpace(from) ? _smtp.SenderEmail : from;

            var message = new MimeMessage();
            var domain = senderEmail.Contains('@') ? senderEmail.Split('@')[1] : "localhost";
            message.MessageId = $"{Guid.NewGuid():N}@{domain}";
            message.From.Add(new MailboxAddress(_smtp.SenderName, senderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            if (!string.IsNullOrWhiteSpace(_smtp.ReplyTo))
                message.ReplyTo.Add(MailboxAddress.Parse(_smtp.ReplyTo));

            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            var secureSocket = _smtp.UseStartTls
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.SslOnConnect;

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtp.Host, _smtp.Port, secureSocket);
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
