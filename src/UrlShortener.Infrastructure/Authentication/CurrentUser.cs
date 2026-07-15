using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UrlShortener.Domain.Users;

namespace UrlShortener.Infrastructure.Authentication;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid Id => Guid.Parse(
        httpContextAccessor.HttpContext?.User.FindFirstValue(
        ClaimTypes.NameIdentifier)!);
}
