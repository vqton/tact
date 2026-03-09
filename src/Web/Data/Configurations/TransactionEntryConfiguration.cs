namespace AccountingVAS.Web.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AccountingVAS.Web.Models;

/// <summary>
/// Entity configuration for TransactionEntry using Fluent API.
/// </summary>
public class TransactionEntryConfiguration : IEntityTypeConfiguration<TransactionEntry>
{
    public void Configure(EntityTypeBuilder<TransactionEntry> builder)
    {
        builder.HasKey(te => te.Id);

        builder.Property(te => te.JournalEntryId)
            .IsRequired();

        builder.Property(te => te.AccountId)
            .IsRequired();

        builder.Property(te => te.Debit)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(te => te.Credit)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(te => te.Description)
            .HasMaxLength(255);

        builder.Property(te => te.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasOne(te => te.JournalEntry)
            .WithMany(je => je.Entries)
            .HasForeignKey(te => te.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(te => te.Account)
            .WithMany(a => a.TransactionEntries)
            .HasForeignKey(te => te.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(te => te.JournalEntryId)
            .HasDatabaseName("ix_transactionentry_journalentryid");

        builder.HasIndex(te => te.AccountId)
            .HasDatabaseName("ix_transactionentry_accountid");
    }
}
