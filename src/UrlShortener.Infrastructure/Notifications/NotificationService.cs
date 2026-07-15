using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Messaging.Users;
using UrlShortener.Infrastructure.Notifications.Templates;

namespace UrlShortener.Infrastructure.Notifications;

internal sealed class NotificationService(IEmailSender emailSender)
    : INotificationService
{
    private static readonly Type _resourceMarkerType = typeof(ResourceMarker);

    public async Task SendProfileImageChangedNotification(
        ProfileImageChangedNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var emailBody = await TemplateReader.ReadTemplateAsync(
            "profile-image-changed.html",
            _resourceMarkerType,
            cancellationToken);

        emailBody = emailBody.Replace("{{USER_NAME}}", request.FirstName);

        await emailSender.SendEmailAsync(
            request.Email,
            "Your Profile Image Has Been Changed",
            emailBody,
            cancellationToken);
    }
}
