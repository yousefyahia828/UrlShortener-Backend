namespace UrlShortener.Abstractions.Authentication;

public interface IPasswordHasher
{
    string HashPassword(string passsword);

    bool Verify(string password, string passwordHash);
}
