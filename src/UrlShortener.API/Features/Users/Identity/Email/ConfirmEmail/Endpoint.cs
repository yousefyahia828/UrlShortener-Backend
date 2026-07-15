using Josephan.CQRS;
using Microsoft.Extensions.Options;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.API.Features.Users.Identity.Email.CheckEmailVerificationToken;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Identity.EmailVerification;

namespace UrlShortener.API.Features.Users.Identity.Email.ConfirmEmail;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("auth/activate-account/{userId}/{id}", async (
            Guid userId,
            Guid id,
            ISender sender,
            IOptions<AppOptions> options) =>
        {
            var isValidLinkResult = await sender.Send(
                new CheckEmailVerificationTokenQuery(userId, id));

            var frontendUrl = options.Value.FrontendUrl;

            // 100% success
            if (!isValidLinkResult.Value)
            {
                return Results.Redirect(frontendUrl + "/invalid-link.html?type=email");
            }

            return await Result
                .From(new ConfirmEmailCommand(userId, id))
                .BindAsync(cmd => sender.Send(cmd))
                .MatchAsync(
                     () => Results.Redirect(frontendUrl + "/email-confirmed.html"),
                    CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName(EmailVerificationLinkFactory.AccountAtivationRouteName)
        .WithSummary("Activate a user account.")
        .WithDescription("""
        Activates a user account using the verification link sent by email.
        
        The verification link contains the user's identifier and a verification token.
        Once the account is successfully activated, the verification token cannot be used again.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
