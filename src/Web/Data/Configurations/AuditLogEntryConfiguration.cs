namespace AccountingVAS.Web.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AccountingVAS.Web.Models;

/// <summary>
/// Entity configuration for AuditLogEntry using Fluent API.
/// </summary>
public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.HasKey(ale => ale.Id);

        builder.Property(ale => ale.UserId)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(ale => ale.Timestamp)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(ale => ale.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ale => ale.EntityId)
            .IsRequired();

        builder.Property(ale => ale.Action)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ale => ale.Changes)
            .HasColumnType("nvarchar(max)");

        builder.Property(ale => ale.IpAddress)
            .HasMaxLength(45);

        // Indexes
        builder.HasIndex(ale => ale.Timestamp)
            .HasDatabaseName("ix_auditlogentry_timestamp");

        builder.HasIndex(ale => ale.UserId)
            .HasDatabaseName("ix_auditlogentry_userid");

        builder.HasIndex(ale => new { ale.EntityType, ale.EntityId })
            .HasDatabaseName("ix_auditlogentry_entity");
    }
}
