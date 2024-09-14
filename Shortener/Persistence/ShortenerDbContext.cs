using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Shortener.Common.Models;

namespace Shortener.Persistence;

public class ShortenerDbContext : DbContext
{
    public DbSet<ShortUrl> ShortUrl { get; set; }

    public ShortenerDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortUrl>()
            .ToCollection("shortUrl");

        modelBuilder.Entity<ShortUrl>()
            .HasIndex(x => x.ShortCode)
            .IsUnique()
            .IsDescending();
    }
}