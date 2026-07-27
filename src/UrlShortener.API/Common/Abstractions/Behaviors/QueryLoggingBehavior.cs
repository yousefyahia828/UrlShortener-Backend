namespace UrlShortener.API.Common.Abstractions.Behaviors;

internal sealed class QueryLoggingBehavior<TQuery, TResponse>(
    ILogger<QueryLoggingBehavior<TQuery, TResponse>> logger)
    : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TQuery request,
        RequestHandlerDelegate<TQuery, TResponse> next,
        CancellationToken cancellationToken)
    {
        var queryName = typeof(TQuery).Name;

        logger.LogInformation("Processing query {Query}", queryName);

        var result = await next(request, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("Completed query {Query}", queryName);
        }
        else
        {
            logger.LogError(
                "Completed query {Query} with errors {@Errors}",
                queryName,
                result.Errors);
        }

        return result;
    }
}
