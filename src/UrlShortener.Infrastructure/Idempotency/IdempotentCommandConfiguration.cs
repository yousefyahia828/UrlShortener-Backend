using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrlShortener.Infrastructure.Idempotency;

internal sealed class IdempotentCommandConfiguration
    : IEntityTypeConfiguration<IdempotentCommand>
{
    public void Configure(EntityTypeBuilder<IdempotentCommand> builder)
    {
        builder.ToTable("idempotent_commands");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name).IsRequired();

        builder.Property(x => x.Response).HasColumnType("JSONB");

        builder.Property(x => x.Status).IsRequired();
    }
}