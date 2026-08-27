using Giromon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giromon.Infrastructure.Persistence.Configurations;

public class WalletTransactionConfiguration
    : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(
        EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.ToTable("wallet_transactions");

        builder.HasKey(transaction => transaction.Id);

        builder.Property(transaction => transaction.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(transaction => transaction.WalletId)
            .HasColumnName("wallet_id")
            .IsRequired();

        builder.Property(transaction => transaction.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(transaction => transaction.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(transaction => transaction.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(transaction => transaction.WalletId);

        builder.HasOne<Wallet>()
            .WithMany()
            .HasForeignKey(transaction => transaction.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}