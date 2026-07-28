using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UrlShortener.Abstractions.Infrastructure;

namespace UrlShortener.Infrastructure.Notifications;

internal sealed class EmailSender(
    IFluentEmail fluentEmail,
    IOptions<EmailSettings> options,
    ILogger<EmailSender> logger) : IEmailSender
{
    private readonly EmailSettings _emailSettings = options.Value;

    public async Task<bool> SendEmailAsync(
        string email,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await fluentEmail
                  .To(email)
                  .Subject(subject)
                  .Body(body, isHtml: true)
                  .SendAsync(cancellationToken);

            if (!response.Successful)
            {
                logger.LogError(
                    "Failed to send email to '{Email}. Errors {@Errors}'",
                    email, response.ErrorMessages);

                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sending email to '{Email}'", email);
            return false;
        }
    }
}
