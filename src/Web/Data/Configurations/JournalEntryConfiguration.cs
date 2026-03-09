namespace AccountingVAS.Web.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AccountingVAS.Web.Models;

/// <summary>
/// Entity configuration for JournalEntry using Fluent API.
/// </summary>
public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(je => je.Id);

        builder.Property(je => je.Date)
            .IsRequired();

        builder.Property(je => je.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(je => je.VoucherRef)
            .HasMaxLength(100);

        builder.Property(je => je.Status)
            .HasConversion<int>();

        builder.Property(je => je.CreatedByUserId)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(je => je.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(je => je.ModifiedByUserId)
            .HasMaxLength(256);

        builder.Property(je => je.ModifiedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasMany(je => je.Entries)
            .WithOne(te => te.JournalEntry)
            .HasForeignKey(te => te.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(je => je.Date)
            .HasDatabaseName("ix_journalentry_date");

        builder.HasIndex(je => je.Status)
            .HasDatabaseName("ix_journalentry_status");

        builder.HasIndex(je => new { je.Date, je.Status })
            .HasDatabaseName("ix_journalentry_date_status");
    }
}
