using Microsoft.Extensions.Options;
using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Domain.Users.Tokens;
using UrlShortener.Infrastructure.Database;
using UrlShortener.Infrastructure.Identity.EmailVerification.Templates;
using UrlShortener.Infrastructure.Notifications;

namespace UrlShortener.Infrastructure.Identity.EmailVerification;

internal sealed class EmailVerificationService(
    EmailVerificationLinkFactory linkFactory,
    IEmailSender emailSender,
    ApplicationDbContext dbContext,
    IOptions<AppOptions> options) : IEmailVerificationService
{
    private static readonly Type _emailResourceMarker = typeof(EmailResourceMarker);
    private readonly AppOptions _options = options.Value;

    public async Task SendAccountActivationLink(
        AccountActivationRequest request,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        var emailVerificationToken = new EmailVerificationToken
        {
            Id = Guid.CreateVersion7(),
            UserId = request.UserId,
            CreatedOnUtc = utcNow,
            ExpiresOnUtc = utcNow.AddDays(1),
        };

        string emailBody = await TemplateReader.ReadTemplateAsync(
            "account-activation-template.html",
            _emailResourceMarker,
            cancellationToken);

        emailBody = emailBody
            .Replace("{{USER_NAME}}", request.FirstName)
            .Replace("{{VERIFICATION_LINK}}", linkFactory.CreateAccountActivationLink(emailVerificationToken));

        var success = await emailSender.SendEmailAsync(
            request.Email,
            "🚀 Activate Your UrlShortener Account",
            emailBody,
            cancellationToken);

        if (!success)
        {
            return;
        }

        await dbContext.EmailVerificationTokens.AddAsync(
            emailVerificationToken,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SendWelcomeEmail(
        WelcomeEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        string emailBody = await TemplateReader.ReadTemplateAsync(
            "account-activated-template.html",
            _emailResourceMarker,
            cancellationToken);

        emailBody = emailBody
            .Replace("{{USER_NAME}}", request.FirstName)
            .Replace("{{LOGIN_LINK}}", $"{_options.FrontendUrl}/index.html");

        await emailSender.SendEmailAsync(
            request.Email,
            "🎉 Your Account Has Been Activated!",
            emailBody,
            cancellationToken);
    }

    public async Task SendEmailChangeConfirmation(
        ChangeEmailConfirmationRequest request,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        var emailVerificationToken = new EmailVerificationToken
        {
            Id = Guid.CreateVersion7(),
            UserId = request.UserId,
            CreatedOnUtc = utcNow,
            ExpiresOnUtc = utcNow.AddMinutes(30)
        };

        string emailBody = await TemplateReader.ReadTemplateAsync(
            "email-change-confirmation.html",
            _emailResourceMarker,
            cancellationToken);

        emailBody = emailBody
            .Replace("{{USER_NAME}}", request.FirstName)
            .Replace("{{VERIFICATION_LINK}}", linkFactory.CreateChangeEmailConfirmationLink(emailVerificationToken));

        var success = await emailSender.SendEmailAsync(
            request.PendingEmail,
            "📧 Confirm Your New Email Address",
            emailBody,
            cancellationToken);

        if (!success)
        {
            return;
        }

        await dbContext.EmailVerificationTokens.AddAsync(
            emailVerificationToken,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SendOldEmailNotification(
        OldEmailNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        string emailBody = await TemplateReader.ReadTemplateAsync(
           "old-email-notification.html",
           _emailResourceMarker,
           cancellationToken);

        emailBody = emailBody.Replace("{{USER_NAME}}", request.FirstName);

        await emailSender.SendEmailAsync(
            request.OldEmailAddress,
            "🔔 Your email address has been changed",
            emailBody,
            cancellationToken);
    }

    public async Task SendNewEmailNotification(
        NewEmailNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        string emailBody = await TemplateReader.ReadTemplateAsync(
           "new-email-linked-notification.html",
           _emailResourceMarker,
           cancellationToken);

        emailBody = emailBody.Replace("{{USER_NAME}}", request.FirstName);

        await emailSender.SendEmailAsync(
            request.NewEmailAddress,
            "✅ Your new email address has been linked",
            emailBody,
            cancellationToken);
    }
}