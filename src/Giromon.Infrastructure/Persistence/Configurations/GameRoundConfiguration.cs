using Giromon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giromon.Infrastructure.Persistence.Configurations;

public class GameRoundConfiguration :
    IEntityTypeConfiguration<GameRound>
{
    public void Configure(EntityTypeBuilder<GameRound> builder)
    {
        builder.ToTable("game_rounds");

        builder.HasKey(gameRound => gameRound.Id);

        builder.Property(gameRound => gameRound.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(gameRound => gameRound.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(gameRound => gameRound.BetAmount)
            .HasColumnName("bet_amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(gameRound => gameRound.FirstSymbol)
            .HasColumnName("first_symbol")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(gameRound => gameRound.SecondSymbol)
            .HasColumnName("second_symbol")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(gameRound => gameRound.ThirdSymbol)
            .HasColumnName("third_symbol")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(gameRound => gameRound.PrizeAmount)
            .HasColumnName("prize_amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(gameRound => gameRound.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(gameRound => gameRound.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(gameRound => gameRound.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}