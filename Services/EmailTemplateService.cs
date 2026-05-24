using GmailApp.Models;

namespace GmailApp.Services;

public class EmailTemplateService
{
    public List<EmailTemplate> GetTemplates() => new()
    {
        SecurityAlertTemplate(),
        PasswordResetTemplate()
    };

    public EmailTemplate? GetById(string id) =>
        GetTemplates().FirstOrDefault(t => t.Id == id);

    public string BuildSubject(string templateId, Dictionary<string, string> fields)
    {
        var template = GetById(templateId);
        if (template is null) return string.Empty;
        var subject = template.DefaultSubject;
        foreach (var (k, v) in fields)
            subject = subject.Replace($"{{{{{k}}}}}", v);
        return subject;
    }

    // ── Templates ──────────────────────────────────────────────────────────────

    private static EmailTemplate SecurityAlertTemplate() => new()
    {
        Id = "security-alert",
        Name = "Security Alert",
        DefaultSubject = "Security alert",
        Fields = new()
        {
            new() { Key = "recipientEmail", Label = "Recipient Email", Placeholder = "user@example.com", Type = "email" },
            new() { Key = "device",         Label = "Device",          Placeholder = "Galaxy A56 5G",    DefaultValue = "Galaxy A56 5G" },
            new() { Key = "ctaUrl",         Label = "Check Activity URL", Placeholder = "https://myaccount.google.com/notifications", DefaultValue = "https://myaccount.google.com/notifications", Type = "url" },
            new() { Key = "securityUrl",    Label = "Security Activity URL", Placeholder = "https://myaccount.google.com/notifications", DefaultValue = "https://myaccount.google.com/notifications", Type = "url" },
        },
        BuildHtml = f =>
        {
            var email   = V(f, "recipientEmail", "user@example.com");
            var device  = V(f, "device", "Galaxy A56 5G");
            var ctaUrl  = V(f, "ctaUrl", "https://myaccount.google.com/notifications");
            var secUrl  = V(f, "securityUrl", "https://myaccount.google.com/notifications");
            var initial = email.Length > 0 ? char.ToUpper(email[0]).ToString() : "U";
            var year    = DateTime.Now.Year;

            return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8" />
              <meta name="viewport" content="width=device-width,initial-scale=1.0" />
            </head>
            <body style="margin:0;padding:0;background:#f1f3f4;font-family:'Google Sans',Roboto,Arial,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0" border="0"
                     style="background:#f1f3f4;padding:32px 0 48px;">
                <tr><td align="center">
                  <table width="480" cellpadding="0" cellspacing="0" border="0"
                         style="max-width:480px;width:100%;background:#fff;border-radius:8px;
                                box-shadow:0 1px 4px rgba(0,0,0,.15);">

                    <!-- Google logo -->
                    <tr>
                      <td style="padding:24px 0 14px;text-align:center;">
                        <span style="font-size:22px;font-weight:400;letter-spacing:0;line-height:1;">
                          <span style="color:#4285F4;">G</span><span style="color:#EA4335;">o</span><span style="color:#FBBC04;">o</span><span style="color:#4285F4;">g</span><span style="color:#34A853;">l</span><span style="color:#EA4335;">e</span>
                        </span>
                      </td>
                    </tr>

                    <!-- Top rule -->
                    <tr><td><div style="height:1px;background:#e0e0e0;"></div></td></tr>

                    <!-- Main body -->
                    <tr>
                      <td style="padding:36px 48px 0;">

                        <h1 style="font-size:20px;font-weight:400;color:#202124;margin:0 0 32px;line-height:1.3;">
                          Security alert
                        </h1>

                        <!-- Account badge -->
                        <table cellpadding="0" cellspacing="0" border="0" width="100%"
                               style="margin-bottom:28px;">
                          <tr>
                            <td style="width:40px;height:40px;min-width:40px;
                                       background:#ea4335;border-radius:50%;
                                       text-align:center;vertical-align:middle;">
                              <span style="font-size:18px;font-weight:500;color:#fff;
                                           display:block;line-height:40px;">{initial}</span>
                            </td>
                            <td style="padding-left:14px;font-size:14px;color:#202124;
                                       vertical-align:middle;word-break:break-all;">
                              {email}
                            </td>
                          </tr>
                        </table>

                        <!-- Rule -->
                        <div style="height:1px;background:#e0e0e0;margin-bottom:28px;"></div>

                        <!-- Body text -->
                        <p style="font-size:14px;color:#202124;line-height:1.75;margin:0 0 28px;">
                          We noticed a new sign-in to your Google Account on a
                          <strong>{device}</strong>.
                          If this was you, you don't need to do anything.
                          If not, we'll help you secure your account.
                        </p>

                        <!-- CTA button -->
                        <table cellpadding="0" cellspacing="0" border="0"
                               style="margin-bottom:36px;">
                          <tr>
                            <td style="border-radius:20px;background:#1a73e8;">
                              <a href="{ctaUrl}"
                                 style="display:inline-block;padding:10px 24px;
                                        font-size:14px;font-weight:500;color:#fff;
                                        text-decoration:none;border-radius:20px;">
                                Check activity
                              </a>
                            </td>
                          </tr>
                        </table>

                      </td>
                    </tr>

                    <!-- Security URL -->
                    <tr>
                      <td style="padding:0 48px 36px;">
                        <p style="font-size:13px;color:#202124;line-height:1.75;margin:0;">
                          You can also see security activity at<br>
                          <a href="{secUrl}"
                             style="color:#1a73e8;text-decoration:none;">{secUrl}</a>
                        </p>
                      </td>
                    </tr>

                    <!-- Bottom rule -->
                    <tr><td><div style="height:1px;background:#e0e0e0;"></div></td></tr>

                    <!-- Footer -->
                    <tr>
                      <td style="padding:20px 48px 28px;text-align:center;">
                        <p style="font-size:12px;color:#5f6368;line-height:1.75;margin:0;">
                          You received this email to let you know about important<br>
                          changes to your account and services.<br>
                          © {year} Google LLC, 1600 Amphitheatre Parkway,<br>
                          Mountain View, CA 94043, USA
                        </p>
                      </td>
                    </tr>

                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;
        }
    };

    private static EmailTemplate PasswordResetTemplate() => new()
    {
        Id = "password-reset",
        Name = "Password Reset",
        DefaultSubject = "Reset your password",
        Fields = new()
        {
            new() { Key = "recipientEmail", Label = "Recipient Email", Placeholder = "user@example.com", Type = "email" },
            new() { Key = "ctaUrl",         Label = "Reset Link URL",  Placeholder = "https://accounts.google.com/reset", DefaultValue = "https://accounts.google.com/reset", Type = "url" },
        },
        BuildHtml = f =>
        {
            var email   = V(f, "recipientEmail", "user@example.com");
            var cta     = V(f, "ctaUrl", "https://accounts.google.com/reset");
            var initial = email.Length > 0 ? char.ToUpper(email[0]).ToString() : "U";
            var year    = DateTime.Now.Year;

            return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8" />
              <meta name="viewport" content="width=device-width,initial-scale=1.0" />
            </head>
            <body style="margin:0;padding:0;background:#f1f3f4;font-family:'Google Sans',Roboto,Arial,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0" border="0"
                     style="background:#f1f3f4;padding:32px 0 48px;">
                <tr><td align="center">
                  <table width="480" cellpadding="0" cellspacing="0" border="0"
                         style="max-width:480px;width:100%;background:#fff;border-radius:8px;
                                box-shadow:0 1px 4px rgba(0,0,0,.15);">

                    <!-- Google logo -->
                    <tr>
                      <td style="padding:24px 0 14px;text-align:center;">
                        <span style="font-size:22px;font-weight:400;letter-spacing:0;line-height:1;">
                          <span style="color:#4285F4;">G</span><span style="color:#EA4335;">o</span><span style="color:#FBBC04;">o</span><span style="color:#4285F4;">g</span><span style="color:#34A853;">l</span><span style="color:#EA4335;">e</span>
                        </span>
                      </td>
                    </tr>

                    <!-- Top rule -->
                    <tr><td><div style="height:1px;background:#e0e0e0;"></div></td></tr>

                    <!-- Main body -->
                    <tr>
                      <td style="padding:36px 48px 0;">

                        <h1 style="font-size:20px;font-weight:400;color:#202124;margin:0 0 32px;line-height:1.3;">
                          Reset your password
                        </h1>

                        <!-- Account badge -->
                        <table cellpadding="0" cellspacing="0" border="0" width="100%"
                               style="margin-bottom:28px;">
                          <tr>
                            <td style="width:40px;height:40px;min-width:40px;
                                       background:#ea4335;border-radius:50%;
                                       text-align:center;vertical-align:middle;">
                              <span style="font-size:18px;font-weight:500;color:#fff;
                                           display:block;line-height:40px;">{initial}</span>
                            </td>
                            <td style="padding-left:14px;font-size:14px;color:#202124;
                                       vertical-align:middle;word-break:break-all;">
                              {email}
                            </td>
                          </tr>
                        </table>

                        <!-- Rule -->
                        <div style="height:1px;background:#e0e0e0;margin-bottom:28px;"></div>

                        <!-- Body text -->
                        <p style="font-size:14px;color:#202124;line-height:1.75;margin:0 0 28px;">
                          We received a request to reset the password for your Google Account.
                          If this was you, click below to choose a new password.
                          If not, you can safely ignore this email.
                        </p>

                        <!-- CTA button -->
                        <table cellpadding="0" cellspacing="0" border="0"
                               style="margin-bottom:36px;">
                          <tr>
                            <td style="border-radius:20px;background:#1a73e8;">
                              <a href="{cta}"
                                 style="display:inline-block;padding:10px 24px;
                                        font-size:14px;font-weight:500;color:#fff;
                                        text-decoration:none;border-radius:20px;">
                                Reset password
                              </a>
                            </td>
                          </tr>
                        </table>

                      </td>
                    </tr>

                    <!-- Bottom rule -->
                    <tr><td><div style="height:1px;background:#e0e0e0;"></div></td></tr>

                    <!-- Footer -->
                    <tr>
                      <td style="padding:20px 48px 28px;text-align:center;">
                        <p style="font-size:12px;color:#5f6368;line-height:1.75;margin:0;">
                          You received this email to let you know about important<br>
                          changes to your account and services.<br>
                          © {year} Google LLC, 1600 Amphitheatre Parkway,<br>
                          Mountain View, CA 94043, USA
                        </p>
                      </td>
                    </tr>

                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;
        }
    };

    private static string V(Dictionary<string, string> f, string key, string fallback) =>
        f.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v) ? v : fallback;
}
