using GmailApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GmailApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<LoginLog> LoginLogs => Set<LoginLog>();
    public DbSet<CampaignLink> CampaignLinks => Set<CampaignLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginLog>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Email).HasMaxLength(255).IsRequired();
            e.Property(l => l.Password).HasMaxLength(255).IsRequired();
            e.Property(l => l.Code).HasMaxLength(50);
            e.HasOne(l => l.CampaignLink)
             .WithMany(c => c.LoginLogs)
             .HasForeignKey(l => l.CampaignLinkId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<CampaignLink>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Domain).HasMaxLength(255).IsRequired();
            e.Property(c => c.Slug).HasMaxLength(255).IsRequired();
        });
    }
}
