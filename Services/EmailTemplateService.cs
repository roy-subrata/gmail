using GmailApp.Models;

namespace GmailApp.Services;

public class EmailTemplateService
{
    public List<EmailTemplate> GetTemplates() => new()
    {
        SecurityAlertTemplate(),
        WelcomeTemplate(),
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
        DefaultSubject = "Security alert for {{recipientEmail}}",
        Fields = new()
        {
            new() { Key = "brandName",    Label = "Brand Name",          Placeholder = "YourBrand",                          DefaultValue = "YourBrand"                         },
            new() { Key = "recipientEmail", Label = "Recipient Email",   Placeholder = "user@example.com", Type = "email"                                                      },
            new() { Key = "device",       Label = "Device / Platform",   Placeholder = "Windows",                            DefaultValue = "Windows"                           },
            new() { Key = "ctaUrl",       Label = "Button URL",          Placeholder = "https://example.com/security",       DefaultValue = "#",          Type = "url"          },
            new() { Key = "securityUrl",  Label = "Security Page URL",   Placeholder = "https://example.com/notifications",  DefaultValue = "https://example.com/notifications", Type = "url" },
        },
        BuildHtml = f =>
        {
            var brand   = V(f, "brandName", "YourBrand");
            var email   = V(f, "recipientEmail", "user@example.com");
            var device  = V(f, "device", "Windows");
            var ctaUrl  = V(f, "ctaUrl", "#");
            var secUrl  = V(f, "securityUrl", "https://example.com/notifications");
            var initial = email.Length > 0 ? char.ToUpper(email[0]).ToString() : "U";
            var year    = DateTime.Now.Year;

            return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8" />
              <meta name="viewport" content="width=device-width,initial-scale=1.0" />
            </head>
            <body style="margin:0;padding:0;background:#f1f3f4;font-family:Arial,Helvetica,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0" border="0" style="background:#f1f3f4;padding:32px 0;">
                <tr><td align="center">
                  <table width="520" cellpadding="0" cellspacing="0" border="0"
                         style="max-width:520px;width:100%;background:#fff;border-radius:8px;
                                box-shadow:0 1px 3px rgba(0,0,0,.12),0 4px 16px rgba(0,0,0,.08);
                                overflow:hidden;">

                    <!-- Alert banner -->
                    <tr>
                      <td style="background:#f8f9fa;padding:14px 28px;border-bottom:1px solid #e8eaed;">
                        <table cellpadding="0" cellspacing="0" border="0" width="100%">
                          <tr>
                            <td style="width:22px;vertical-align:top;padding-top:1px;font-size:17px;">&#9993;</td>
                            <td style="padding-left:10px;font-size:12.5px;color:#555;line-height:1.5;">
                              This is a copy of a security alert sent to
                              <strong style="color:#202124;">{email}</strong>.
                              If you don't recognise this activity, secure your account immediately.
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>

                    <!-- Brand + heading -->
                    <tr>
                      <td style="padding:36px 48px 24px;text-align:center;">
                        <div style="font-size:26px;font-weight:700;color:#202124;letter-spacing:-0.5px;margin-bottom:22px;">
                          {brand}
                        </div>
                        <h1 style="font-size:24px;font-weight:700;color:#202124;margin:0 0 18px;line-height:1.25;">
                          A new sign&#8209;in on {device}
                        </h1>
                        <!-- User badge -->
                        <table cellpadding="0" cellspacing="0" border="0" style="margin:0 auto 4px;">
                          <tr>
                            <td style="width:32px;height:32px;background:#e8710a;border-radius:50%;
                                       text-align:center;vertical-align:middle;">
                              <span style="font-size:14px;font-weight:700;color:#fff;line-height:32px;">{initial}</span>
                            </td>
                            <td style="padding-left:10px;font-size:14px;color:#3c4043;vertical-align:middle;">
                              {email}
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>

                    <!-- Divider -->
                    <tr>
                      <td style="padding:0 48px;">
                        <div style="height:1px;background:#e8eaed;"></div>
                      </td>
                    </tr>

                    <!-- Body + CTA -->
                    <tr>
                      <td style="padding:26px 48px 36px;text-align:center;">
                        <p style="font-size:15px;color:#444;line-height:1.7;margin:0 0 28px;">
                          We noticed a new sign&#8209;in to your account on a
                          <strong>{device}</strong> device.
                          If this was you, you don't need to do anything.
                          If not, we'll help you secure your account.
                        </p>
                        <a href="{ctaUrl}"
                           style="display:inline-block;background:#1a73e8;color:#fff;
                                  text-decoration:none;padding:13px 36px;
                                  border-radius:4px;font-size:15px;font-weight:500;">
                          Check activity
                        </a>
                      </td>
                    </tr>

                    <!-- Security URL footer -->
                    <tr>
                      <td style="padding:18px 48px;border-top:1px solid #e8eaed;text-align:center;">
                        <p style="font-size:12px;color:#777;margin:0 0 4px;">
                          You can also see security activity at
                        </p>
                        <p style="font-size:12px;color:#1a73e8;margin:0;">{secUrl}</p>
                      </td>
                    </tr>

                    <!-- Copyright -->
                    <tr>
                      <td style="padding:14px 48px 24px;background:#f8f9fa;
                                 border-top:1px solid #e8eaed;text-align:center;">
                        <p style="font-size:11px;color:#999;margin:0 0 4px;">
                          You received this email to let you know about important changes to your account and services.
                        </p>
                        <p style="font-size:11px;color:#999;margin:0;">© {year} {brand}</p>
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

    private static EmailTemplate WelcomeTemplate() => new()
    {
        Id = "welcome",
        Name = "Welcome Email",
        DefaultSubject = "Welcome to {{brandName}}, {{recipientName}}!",
        Fields = new()
        {
            new() { Key = "brandName",     Label = "Brand Name",     Placeholder = "YourBrand",          DefaultValue = "YourBrand" },
            new() { Key = "recipientName", Label = "Recipient Name", Placeholder = "John",               DefaultValue = "there"     },
            new() { Key = "ctaUrl",        Label = "Get Started URL", Placeholder = "https://example.com", DefaultValue = "#", Type = "url" },
        },
        BuildHtml = f =>
        {
            var brand = V(f, "brandName", "YourBrand");
            var name  = V(f, "recipientName", "there");
            var cta   = V(f, "ctaUrl", "#");
            var year  = DateTime.Now.Year;

            return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head><meta charset="utf-8"/></head>
            <body style="margin:0;padding:0;background:#f1f3f4;font-family:Arial,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0" border="0" style="background:#f1f3f4;padding:32px 0;">
                <tr><td align="center">
                  <table width="520" cellpadding="0" cellspacing="0" border="0"
                         style="max-width:520px;background:#fff;border-radius:8px;
                                box-shadow:0 1px 3px rgba(0,0,0,.12),0 4px 16px rgba(0,0,0,.08);overflow:hidden;">
                    <tr>
                      <td style="background:#1a73e8;padding:32px 48px;text-align:center;">
                        <div style="font-size:26px;font-weight:700;color:#fff;">{brand}</div>
                      </td>
                    </tr>
                    <tr>
                      <td style="padding:40px 48px 32px;text-align:center;">
                        <div style="font-size:48px;margin-bottom:16px;">&#127881;</div>
                        <h1 style="font-size:24px;font-weight:700;color:#202124;margin:0 0 16px;">
                          Welcome, {name}!
                        </h1>
                        <p style="font-size:15px;color:#444;line-height:1.7;margin:0 0 28px;">
                          We're thrilled to have you on board. Your account is ready.
                          Click below to get started and explore everything we have to offer.
                        </p>
                        <a href="{cta}"
                           style="display:inline-block;background:#1a73e8;color:#fff;
                                  text-decoration:none;padding:13px 36px;
                                  border-radius:4px;font-size:15px;font-weight:500;">
                          Get Started
                        </a>
                      </td>
                    </tr>
                    <tr>
                      <td style="padding:16px 48px 24px;background:#f8f9fa;
                                 border-top:1px solid #e8eaed;text-align:center;">
                        <p style="font-size:11px;color:#999;margin:0;">© {year} {brand}. All rights reserved.</p>
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
        DefaultSubject = "Reset your {{brandName}} password",
        Fields = new()
        {
            new() { Key = "brandName",     Label = "Brand Name",      Placeholder = "YourBrand", DefaultValue = "YourBrand" },
            new() { Key = "recipientName", Label = "Recipient Name",  Placeholder = "John",      DefaultValue = "User"      },
            new() { Key = "ctaUrl",        Label = "Reset Link URL",  Placeholder = "https://example.com/reset", DefaultValue = "#", Type = "url" },
            new() { Key = "expiryHours",   Label = "Link Expiry (hrs)", Placeholder = "24",      DefaultValue = "24"        },
        },
        BuildHtml = f =>
        {
            var brand   = V(f, "brandName", "YourBrand");
            var name    = V(f, "recipientName", "User");
            var cta     = V(f, "ctaUrl", "#");
            var expiry  = V(f, "expiryHours", "24");
            var year    = DateTime.Now.Year;

            return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head><meta charset="utf-8"/></head>
            <body style="margin:0;padding:0;background:#f1f3f4;font-family:Arial,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0" border="0" style="background:#f1f3f4;padding:32px 0;">
                <tr><td align="center">
                  <table width="520" cellpadding="0" cellspacing="0" border="0"
                         style="max-width:520px;background:#fff;border-radius:8px;
                                box-shadow:0 1px 3px rgba(0,0,0,.12),0 4px 16px rgba(0,0,0,.08);overflow:hidden;">
                    <tr>
                      <td style="padding:36px 48px 24px;text-align:center;">
                        <div style="font-size:26px;font-weight:700;color:#202124;margin-bottom:24px;">{brand}</div>
                        <div style="font-size:44px;margin-bottom:16px;">&#128274;</div>
                        <h1 style="font-size:22px;font-weight:700;color:#202124;margin:0 0 16px;">
                          Reset your password
                        </h1>
                        <p style="font-size:15px;color:#444;line-height:1.7;margin:0 0 8px;">
                          Hi {name}, we received a request to reset your password.
                        </p>
                        <p style="font-size:15px;color:#444;line-height:1.7;margin:0 0 28px;">
                          Click below to choose a new password. This link expires in <strong>{expiry} hours</strong>.
                        </p>
                        <a href="{cta}"
                           style="display:inline-block;background:#d93025;color:#fff;
                                  text-decoration:none;padding:13px 36px;
                                  border-radius:4px;font-size:15px;font-weight:500;">
                          Reset Password
                        </a>
                        <p style="font-size:13px;color:#777;margin:24px 0 0;">
                          If you didn't request this, you can safely ignore this email.
                        </p>
                      </td>
                    </tr>
                    <tr>
                      <td style="padding:16px 48px 24px;background:#f8f9fa;
                                 border-top:1px solid #e8eaed;text-align:center;">
                        <p style="font-size:11px;color:#999;margin:0;">© {year} {brand}. All rights reserved.</p>
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
