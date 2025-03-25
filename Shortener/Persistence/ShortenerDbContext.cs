using MongoDB.EntityFrameworkCore.Extensions;

namespace Shortener.Persistence
{
    public class ShortenerDbContext(DbContextOptions<ShortenerDbContext> options) : DbContext(options)
    {
        public DbSet<Url> Urls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Url>(c =>
            {
                c.ToCollection("Urls");

                c.HasKey(u => u.Id);
                c.Property(u => u.Id)
                    .IsUnicode()
                    .IsRequired()
                    .HasMaxLength(36);

                c.Property(u => u.LongUrl)
                    .IsRequired()
                    .HasMaxLength(2048);

                c.Property(u => u.ShortCode)
                    .IsRequired()
                    .HasMaxLength(20);

                c.Property(u => u.CreatedAt)
                    .IsRequired();

                c.Property(u => u.ExpiresAt)
                    .IsRequired();

                c.HasIndex(u => u.ShortCode)
                    .IsUnique()
                    .IsDescending();
            });
        }
    }
}