using Josephan.CQRS;
using Microsoft.Extensions.Options;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.API.Features.Users.Identity.Email.CheckEmailVerificationToken;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Identity.EmailVerification;

namespace UrlShortener.API.Features.Users.Identity.Email.ConfirmEmailChange;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/confirm-email-change/{userId}/{id}", async (
            Guid userId,
            Guid id,
            ISender sender,
            IOptions<AppOptions> options) =>
        {
            var query = new CheckEmailVerificationTokenQuery(userId, id);

            var result = await sender.Send(query);

            var frontendUrl = options.Value.FrontendUrl;

            // 100% success
            if (!result.Value)
            {
                return Results.Redirect(frontendUrl + "/invalid-link.html?type=email");
            }

            return await Result
                .From(new ConfirmEmailChangeCommand(userId, id))
                .BindAsync(cmd => sender.Send(cmd))
                .MatchAsync(
                    () => Results.Redirect(options.Value.FrontendUrl + "/email-confirmed.html"),
                    CustomResults.Problem);
        })
        .WithName(EmailVerificationLinkFactory.EmailChangeRouteName)
        .WithTags(Tags.Users)
        .WithSummary("Confirm an email address change.")
        .WithDescription("""
        Confirms the pending email address change using the confirmation link sent to the new email address.
        
        The email address is updated only after the confirmation link is successfully validated.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .AllowAnonymous();
    }
}
