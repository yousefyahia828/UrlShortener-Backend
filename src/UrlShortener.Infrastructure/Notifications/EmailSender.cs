using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;
using UrlShortener.Abstractions.Infrastructure;

namespace UrlShortener.Infrastructure.Notifications;

internal sealed class EmailSender(
    //IFluentEmail fluentEmail,
    IResend resend,
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
            //var response = await fluentEmail
            //    .To(email)
            //    .Subject(subject)
            //    .Body(body, isHtml: true)
            //    .SendAsync(cancellationToken);

            //if (!response.Successful)
            //{
            //    using (LogContext.PushProperty("errors", response.ErrorMessages, true))
            //    {
            //        logger.LogError("Failed to send email to '{Email}'", email);
            //    }
            //}

            //return response.Successful;

            await resend.EmailSendAsync(
                new EmailMessage()
                {
                    From = _emailSettings.SenderEmail,
                    To = [email],
                    Subject = subject,
                    HtmlBody = body,
                },
                cancellationToken);


            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sending email to '{Email}'", email);
            return false;
        }
    }
}
