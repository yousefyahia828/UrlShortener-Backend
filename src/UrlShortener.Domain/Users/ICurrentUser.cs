namespace UrlShortener.Domain.Users;

public interface ICurrentUser
{
    Guid Id { get; }
}
