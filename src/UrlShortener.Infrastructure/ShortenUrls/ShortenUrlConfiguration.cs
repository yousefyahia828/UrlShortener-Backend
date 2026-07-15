using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.ShortenUrls;

namespace UrlShortener.Infrastructure.ShortenUrls;

internal sealed class ShortenUrlConfiguration : IEntityTypeConfiguration<ShortenUrl>
{
    public void Configure(EntityTypeBuilder<ShortenUrl> builder)
    {
        builder.ToTable("shorten_urls", "shorten");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Enabled).HasDefaultValue(true);

        builder.Property(x => x.LongUrl).IsRequired();

        builder.Property(x => x.Code).HasMaxLength(8);

        builder.Property(x => x.Description).HasMaxLength(100);

        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasIndex(x => x.LongUrl);

        builder.HasIndex(x => new { x.UserId, x.Enabled, x.CreatedOnUtc });
    }
}
