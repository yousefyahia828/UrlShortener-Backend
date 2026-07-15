using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using UrlShortener.Abstractions.Infrastructure;

namespace UrlShortener.Infrastructure.Notifications;

internal sealed class EmailSender(
    IFluentEmail fluentEmail,
    ILogger<EmailSender> logger) : IEmailSender
{
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
                using (LogContext.PushProperty("errors", response.ErrorMessages, true))
                {
                    logger.LogError("Failed to send email to '{Email}'", email);
                }
            }

            return response.Successful;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sending email to '{Email}'", email);
            return false;
        }
    }
}
