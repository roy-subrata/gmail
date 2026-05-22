using GmailApp.Components;
using GmailApp.Data;
using GmailApp.Models;
using GmailApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(SmtpSettings.Section));
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
app.UseAntiforgery();
app.MapGet("/health", () => Results.Ok());
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
