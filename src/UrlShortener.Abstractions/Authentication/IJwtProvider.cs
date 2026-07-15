using UrlShortener.Domain.Users;

namespace UrlShortener.Abstractions.Authentication;

public interface IJwtProvider
{
    (string Token, DateTime ExpiresOnUtc) GenerateToken(User user);
}
