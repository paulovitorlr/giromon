using Giromon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giromon.Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("wallets");

        builder.HasKey(wallet => wallet.Id);

        builder.Property(wallet => wallet.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(wallet => wallet.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(wallet => wallet.Balance)
            .HasColumnName("balance")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(wallet => wallet.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(wallet => wallet.UserId)
            .IsUnique();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Wallet>(wallet => wallet.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}