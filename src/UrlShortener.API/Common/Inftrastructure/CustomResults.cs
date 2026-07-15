using Josephan.CQRS;

namespace UrlShortener.API.Common.Inftrastructure;

public sealed record ErrorResponse(string Code, string? Field, string Description);

public static class CustomResults
{
    public static IResult Problem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException(
                "Could not convert a successful result to a problem response.");
        }

        var errorType = result.Errors.First().Type;

        return Results.Problem(
            statusCode: GetStatusCode(errorType),
            type: GetType(errorType),
            title: GetTitle(errorType),
            extensions: GetErrors(result.Errors));
    }

    static string GetTitle(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation =>
                "One or more validation errors occurred",

            ErrorType.Failure =>
                "The request could not be processed",

            ErrorType.NotFound =>
                "The requested resource was not found",

            ErrorType.Conflict =>
                "The request conflicts with the current state of the resource",

            ErrorType.Unauthorized =>
                "Authentication is required to access this resource",

            ErrorType.Forbidden =>
                "You do not have permission to perform this action",

            _ =>
                "An unexpected error occurred"
        };

    static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation or ErrorType.Failure =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.1",

            ErrorType.NotFound =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",

            ErrorType.Conflict =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.8",

            ErrorType.Unauthorized =>
                "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",

            ErrorType.Forbidden =>
                "https://tools.ietf.org/html/rfc7231#section-6.5.3",

            _ =>
                "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

    static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation or ErrorType.Failure => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

    static Dictionary<string, object?> GetErrors(IEnumerable<Error> errors)
    {
        return new Dictionary<string, object?>
        {
            ["errors"] = errors
                .Select(e => new ErrorResponse(e.Code, ExtractField(e.Code, e.Type), e.Description))
                .OrderBy(e => e.Code)
        };
    }

    static string? ExtractField(string code, ErrorType errorType)
    {
        if (errorType is not ErrorType.Validation) return null;

        var field = code.Split('.').First();
        var camelCaseField = char.ToLower(field[0]) + field[1..];

        return camelCaseField;
    }
}