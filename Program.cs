using GmailApp.Components;
using GmailApp.Data;
using GmailApp.Models;
using GmailApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(SmtpSettings.Section));
builder.Services.Configure<DashboardAuthSettings>(builder.Configuration.GetSection(DashboardAuthSettings.Section));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name     = "dash_auth";
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Strict;
        o.ExpireTimeSpan  = TimeSpan.FromDays(7);
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

builder.Services.AddSingleton<AppStateService>();
builder.Services.AddScoped<LoginLogService>();
builder.Services.AddScoped<CampaignLinkService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddSingleton<EmailTemplateService>();

var app = builder.Build();

// Create SQLite database and all tables from the current model on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    // Add IsVisited column to existing databases that predate this field
    var cols = db.Database.SqlQueryRaw<string>("SELECT name FROM pragma_table_info('CampaignLinks')").ToList();
    if (!cols.Contains("IsVisited"))
        db.Database.ExecuteSqlRaw("ALTER TABLE \"CampaignLinks\" ADD COLUMN \"IsVisited\" INTEGER NOT NULL DEFAULT 0");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// In a container a reverse proxy handles TLS — skip the redirect there
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
    app.UseHttpsRedirection();

app.UseWebSockets();   // must be before MapRazorComponents so SignalR can upgrade to WS
app.UseAuthentication();
app.UseAuthorization();

// Guard dashboard routes; redirect authenticated users away from login page
app.Use(async (ctx, next) =>
{
    var path = ctx.Request.Path.Value ?? "";
    var authed = ctx.User.Identity?.IsAuthenticated ?? false;

    var isProtected = path == "/"
        || path.Equals("/dashboard",       StringComparison.OrdinalIgnoreCase)
        || path.Equals("/email-dashboard", StringComparison.OrdinalIgnoreCase);

    if (isProtected && !authed)
    {
        ctx.Response.Redirect("/admin-login?returnUrl=" + Uri.EscapeDataString(path));
        return;
    }

    // Already signed in — skip the login page
    if (path.Equals("/admin-login", StringComparison.OrdinalIgnoreCase) && authed)
    {
        ctx.Response.Redirect("/");
        return;
    }

    await next();
});

app.UseAntiforgery();

// ── Auth endpoints ──────────────────────────────────────────────────────────

app.MapPost("/do-login", async (HttpContext ctx, IOptions<DashboardAuthSettings> opts) =>
{
    var form = await ctx.Request.ReadFormAsync();
    var username  = form["username"].ToString().Trim();
    var password  = form["password"].ToString();
    var returnUrl = form["returnUrl"].ToString();
    var s = opts.Value;

    if (username == s.Username && password == s.Password)
    {
        var claims   = new[] { new Claim(ClaimTypes.Name, username) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await ctx.SignInAsync(new ClaimsPrincipal(identity));
        return Results.Redirect(returnUrl.StartsWith("/") ? returnUrl : "/");
    }

    var errUrl = "/admin-login?error=1";
    if (returnUrl.StartsWith("/"))
        errUrl += "&returnUrl=" + Uri.EscapeDataString(returnUrl);
    return Results.Redirect(errUrl);
}).DisableAntiforgery();

app.MapGet("/do-logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/admin-login");
});

// ── App endpoints ───────────────────────────────────────────────────────────

app.MapGet("/health", () => Results.Ok());
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
