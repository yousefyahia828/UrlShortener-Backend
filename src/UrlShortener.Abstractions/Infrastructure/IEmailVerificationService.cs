using UrlShortener.Abstractions.Messaging.Users;

namespace UrlShortener.Abstractions.Infrastructure;

public interface IEmailVerificationService
{
    Task SendAccountActivationLink(
        AccountActivationRequest request,
        CancellationToken cancellationToken = default);

    Task SendWelcomeEmail
        (WelcomeEmailRequest request,
        CancellationToken cancellationToken = default);

    Task SendEmailChangeConfirmation(
       ChangeEmailConfirmationRequest request,
       CancellationToken cancellationToken = default);

    Task SendOldEmailNotification(
        OldEmailNotificationRequest request,
        CancellationToken cancellationToken = default);

    Task SendNewEmailNotification(
        NewEmailNotificationRequest request,
        CancellationToken cancellationToken = default);
}