using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Library.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связи многие-ко-многим между Book и Author
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Authors)
            .WithMany(a => a.Books)
            .UsingEntity(j => j.ToTable("BookAuthors"));

        // Настройка связи многие-ко-многим между Book и Genre
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Genres)
            .WithMany(g => g.Books)
            .UsingEntity(j => j.ToTable("BookGenres"));

        // Настройка Book
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
            entity.Property(b => b.ISBN).IsRequired().HasMaxLength(13);
        });

        // Настройка Author
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(a => a.LastName).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Country).HasMaxLength(100);
        });

        // Настройка Genre
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(50);
            entity.Property(g => g.Description).HasMaxLength(255);
        });
    }
}