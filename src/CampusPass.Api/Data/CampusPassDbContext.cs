using CampusPass.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPass.Api.Data;

public class CampusPassDbContext : DbContext
{
    public CampusPassDbContext(DbContextOptions<CampusPassDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(5000)
                .IsRequired();

            entity.Property(e => e.StartsAt).IsRequired();
            entity.Property(e => e.Capacity).IsRequired();
            entity.Property(e => e.RegisteredCount).IsRequired();
            entity.Property(e => e.AuthorId).IsRequired();

            entity.Property(e => e.State)
                .HasConversion<string>()
                .IsRequired();

            entity.HasIndex(e => new { e.State, e.StartsAt });
            entity.HasIndex(e => e.AuthorId);
        });
    }
}
