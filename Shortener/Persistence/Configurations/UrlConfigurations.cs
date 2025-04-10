using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using Shortener.Infrastructure;

namespace Shortener.Persistence.Configurations;

public class UrlConfigurations : IEntityTypeConfiguration<Url>
{
    public void Configure(EntityTypeBuilder<Url> builder)
    {
        builder.ToCollection("Urls");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(u => u.LongUrl)
            .IsUnicode()
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(u => u.ShortCode)
            .IsUnicode(false)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.ExpiresAt)
            .HasDateTimeKind(DateTimeKind.Utc)
            .IsRequired();

        builder.HasIndex(u => u.ShortCode)
            .IsUnique()
            .IsDescending();

        builder.HasQueryFilter(x => x.RemovedAt == null);
        builder.MapAuditableColumns();
    }
}