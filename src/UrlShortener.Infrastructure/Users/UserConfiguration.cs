using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Users;

namespace UrlShortener.Infrastructure.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "auth");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.FirstName).HasMaxLength(20).IsRequired();

        builder.Property(x => x.LastName).HasMaxLength(20).IsRequired();

        builder.Property(x => x.Email).HasMaxLength(255).IsRequired();

        builder.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();

        builder.Property(x => x.EmailConfirmed).HasDefaultValue(false);

        builder.Property(x => x.ProfileImageUrl).HasMaxLength(200);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasIndex(x => new { x.FirstName, x.LastName });

        builder.HasMany(x => x.ShortenUrls)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
