using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Shortener.Common.Models;

namespace Shortener.Persistence;

public class ShortenerDbContext : DbContext
{
    public DbSet<ShortUrls> ShortUrls { get; set; }

    public ShortenerDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShortUrls>().ToCollection("shortUrls");
    }
}