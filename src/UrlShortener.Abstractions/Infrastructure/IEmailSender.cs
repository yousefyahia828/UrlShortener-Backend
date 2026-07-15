namespace UrlShortener.Abstractions.Infrastructure;

public interface IEmailSender
{
    Task<bool> SendEmailAsync(
        string Email,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}
