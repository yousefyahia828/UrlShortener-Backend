using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UrlShortener.Abstractions.Authentication;
using UrlShortener.Domain.Users;

namespace UrlShortener.Infrastructure.Authentication.Jwt;

internal sealed class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = options.Value;

    public (string Token, DateTime ExpiresOnUtc) GenerateToken(User user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var expiresOnUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.LifetimeInMinutes);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
        ];

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signingCredentials,
            Expires = expiresOnUtc // igonre gap 
        };

        var handler = new JwtSecurityTokenHandler();

        var securityToken = handler.CreateToken(tokenDescriptor);

        return (handler.WriteToken(securityToken), expiresOnUtc);
    }
}
