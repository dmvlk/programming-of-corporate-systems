using Microsoft.EntityFrameworkCore;
using NetworkAnalyzer.Models;
namespace NetworkAnalyzer.Data;

public class AppDbContext : DbContext
{
    public DbSet<UrlHistory> UrlHistories { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=NetworkAnalyzer.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.CheckTime).IsRequired();
        });
    }
}