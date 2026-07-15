using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Users.Tokens;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens", "auth");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.TokenHash)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.ExpiredOnUtc).IsRequired();
        builder.Property(x => x.RevokedOnUtc).IsRequired(false);
        builder.Property(x => x.ReplacedByTokenId).IsRequired(false);

        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => x.UserId);

        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.IsActive);
        builder.Ignore(x => x.IsRevoked);

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<RefreshToken>()
               .WithMany()
               .HasForeignKey(x => x.ReplacedByTokenId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}