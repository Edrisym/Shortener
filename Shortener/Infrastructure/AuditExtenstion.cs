using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shortener.Infrastructure;

public static class AuditExtenstion
{
    public static void MapAuditableColumns(this EntityTypeBuilder modelBuilder)
    {
        modelBuilder.MapCreatableColumns();
        modelBuilder.MapModifiableColumns();
        modelBuilder.MapRemovableColumns();
    }

    private static void MapCreatableColumns(this EntityTypeBuilder modelBuilder)
    {
        modelBuilder.Property("CreatedAt")
            .HasDateTimeKind(DateTimeKind.Utc)
            .IsRequired();

        modelBuilder.Property("CreatedBy")
            .HasMaxLength(64)
            .IsRequired(false);
    }

    private static void MapModifiableColumns(this EntityTypeBuilder modelBuilder)
    {
        modelBuilder.Property("ModifiedAt")
            .HasDateTimeKind(DateTimeKind.Utc)
            .IsRequired(false);

        modelBuilder.Property("ModifiedBy")
            .HasMaxLength(64)
            .IsRequired(false);
    }

    private static void MapRemovableColumns(this EntityTypeBuilder modelBuilder)
    {
        modelBuilder.Property("RemovedAt")
            .HasDateTimeKind(DateTimeKind.Utc)
            .IsRequired(false);

        modelBuilder.Property("RemovedBy")
            .HasMaxLength(64)
            .IsRequired(false);
    }
}