using Josephan.CQRS;
using Microsoft.Extensions.Logging;
using Serilog.Context;

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
            using (LogContext.PushProperty("Errors", result.Errors, true))
            {
                logger.LogError("Completed query {Query} with errors", queryName);
            }
        }

        return result;
    }
}
