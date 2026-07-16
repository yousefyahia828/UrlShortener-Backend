using Josephan.CQRS;
using UrlShortener.Domain.Primitives;
using UrlShortener.Domain.ShortenUrls;
using UrlShortener.Domain.Users.Events;

namespace UrlShortener.Domain.Users;

public sealed class User : Entity<Guid>, IAuditableEntity
{
    private User()
    {
        // EF Core
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool EmailConfirmed { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public string? PendingEmail { get; private set; }
    public DateTime? EmailPendedOnUtc { get; private set; }
    public string ProfileImageUrl { get; private set; }

    public ICollection<ShortenUrl> ShortenUrls { get; private set; } = null!;

    public static User Register(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        string profileImageUrl)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = passwordHash,
            EmailConfirmed = false,
            ProfileImageUrl = profileImageUrl
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id, user.Email, user.FirstName));

        return user;
    }

    public void ConfirmEmail()
    {
        if (EmailConfirmed) return;

        EmailConfirmed = true;

        Raise(new UserEmailConfirmedDomainEvent(Id, Email, FirstName));
    }

    public void RequestEmailVerification()
    {
        if (EmailConfirmed) return;

        Raise(new UserEmailConfirmationRequestedDomainEvent(Id, Email, FirstName));
    }

    public Result UpdateProfile(string firstName, string lastName)
    {
        if (FirstName == firstName && LastName == lastName)
        {
            return UserErrors.SameName;
        }

        FirstName = firstName;
        LastName = lastName;

        Raise(new UserProfileUpdatedDomainEvent(Id));

        return Unit.Value;
    }

    public void UpdateProfileImage(string imageUrl)
    {
        ProfileImageUrl = imageUrl;

        Raise(new ProfileImageUpdatedDomainEvent(Id, Email, FirstName));
    }

    public Result RequestEmailChange(string newEmail)
    {
        if (Email == newEmail)
        {
            return UserErrors.SameEmail;
        }

        PendingEmail = newEmail;
        EmailPendedOnUtc = DateTime.UtcNow;

        Raise(new UserEmailChangeRequestedDomainEvent(Id, PendingEmail, FirstName));

        return Unit.Value;
    }

    public Result ConfirmEmailChange()
    {
        if (PendingEmail is null)
        {
            return UserErrors.NoPendingEmailChange;
        }

        var oldEmail = Email;

        Email = PendingEmail;
        PendingEmail = null;
        EmailPendedOnUtc = null;

        Raise(new UserEmailChangedDomainEvent(Id, oldEmail, Email, FirstName));

        return Unit.Value;
    }

    public Result CancelChangeEmailRequest()
    {
        if (PendingEmail is null)
        {
            return UserErrors.NoPendingEmailChange;
        }

        PendingEmail = null;
        EmailPendedOnUtc = null;

        Raise(new UserEmailChangeCancelledDomainEvent(Id));

        return Unit.Value;
    }

    public void RequestPasswordChange()
    {
        if (EmailConfirmed)
        {
            Raise(new UserPasswordResetRequestedDomainEvent(Id, Email, FirstName));
        }
    }

    public Result ResetPassword(string newPasswordHash)
    {
        if (PasswordHash.SequenceEqual(newPasswordHash))
        {
            return UserErrors.SamePassword;
        }

        PasswordHash = newPasswordHash;

        Raise(new UserPasswordHasBeenResetDomainEvent(Id, Email, FirstName));

        return Unit.Value;
    }

    public Result ChangePassword(string newPasswordHash)
    {
        if (PasswordHash.SequenceEqual(newPasswordHash))
        {
            return UserErrors.SamePassword;
        }

        PasswordHash = newPasswordHash;

        Raise(new UserPasswordChangedDomainEvent(Id, Email, FirstName));

        return Unit.Value;
    }
}