namespace AccountingVAS.Web.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AccountingVAS.Web.Models;

/// <summary>
/// Entity configuration for Account using Fluent API.
/// Defines primary keys, constraints, indexes, and mappings to database.
/// </summary>
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Code)
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Type)
            .HasConversion<int>();

        builder.Property(a => a.Level)
            .HasDefaultValue(1);

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true);

        builder.Property(a => a.Balance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(a => a.ModifiedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasOne(a => a.Parent)
            .WithMany()
            .HasForeignKey(a => a.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.TransactionEntries)
            .WithOne(te => te.Account)
            .HasForeignKey(te => te.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for query performance
        builder.HasIndex(a => a.Code)
            .IsUnique()
            .HasDatabaseName("ix_account_code");

        builder.HasIndex(a => a.ParentId)
            .HasDatabaseName("ix_account_parentid");

        builder.HasIndex(a => a.IsActive)
            .HasDatabaseName("ix_account_isactive");
    }
}
