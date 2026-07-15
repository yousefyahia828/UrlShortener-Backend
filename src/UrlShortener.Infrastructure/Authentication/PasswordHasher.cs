using UrlShortener.Abstractions.Authentication;

namespace UrlShortener.Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string passsword)
    {
        return BCrypt.Net.BCrypt.HashPassword(passsword);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
