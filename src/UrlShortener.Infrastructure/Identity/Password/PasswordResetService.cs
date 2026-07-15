using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.Abstractions.Authentication;
using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users.Tokens;
using UrlShortener.Infrastructure.Identity.Password.Templates;
using UrlShortener.Infrastructure.Notifications;

namespace UrlShortener.Infrastructure.Identity.Password;

internal sealed class PasswordResetService(
    ResetPasswordTokenLinkFactory linkFactory,
    IEmailSender emailSender,
    IApplicationDbContext dbContext)
    : IPasswordResetService
{
    private static readonly Type _passwordResourceMarker = typeof(PasswordResourceMarker);
    public async Task SendPasswordResetEmail(
        PasswordResetRequest request,
        CancellationToken cancellationToken = default)
    {
        string rawToken = GetRawToken();

        var utcNow = DateTime.UtcNow;

        var token = new PasswordResetToken
        {
            Id = Guid.CreateVersion7(),
            UserId = request.UserId,
            TokenHash = HashToken(rawToken),
            CreatedOnUtc = utcNow,
            ExpiresOnUtc = utcNow.AddMinutes(30),
        };

        string emailBody = await TemplateReader.ReadTemplateAsync(
            "password-reset-template.html",
            _passwordResourceMarker,
            cancellationToken);

        emailBody = emailBody
            .Replace("{{USER_NAME}}", request.FirstName)
            .Replace("{{RESET_PASSWORD_LINK}}", linkFactory.CreatePasswordResetLink(rawToken));

        var success = await emailSender.SendEmailAsync(
            request.Email,
            "🔒 Reset Your Password",
            emailBody,
            cancellationToken);

        if (success)
        {
            await dbContext.PasswordResetTokens.AddAsync(token, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SendPasswordChangedNotification(
        PasswordChangedNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var emailBody = await TemplateReader.ReadTemplateAsync(
            "password-changed-template.html",
            _passwordResourceMarker,
            cancellationToken);

        emailBody = emailBody.Replace("{{USER_NAME}}", request.FirstName);

        await emailSender.SendEmailAsync(
            request.Email,
            "🔒 Your Password Has Been Changed!",
            emailBody,
            cancellationToken);
    }

    public string HashToken(string rawToken)
        => Base64Url.EncodeToString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

    private static string GetRawToken()
        => Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));
}
