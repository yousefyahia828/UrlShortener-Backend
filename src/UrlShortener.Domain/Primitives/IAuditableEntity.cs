namespace UrlShortener.Domain.Primitives;

public interface IAuditableEntity
{
    DateTime CreatedOnUtc { get; }
}
