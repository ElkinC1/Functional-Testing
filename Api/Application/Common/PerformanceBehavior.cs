using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Api.Application.Common;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        var requestName = typeof(TRequest).Name;

        if (elapsedMilliseconds > 500)
        {
            _logger.LogWarning(
                "Solicitud de larga duración detectada: {Name} ({ElapsedMilliseconds} milisegundos) {@Request}",
                requestName,
                elapsedMilliseconds,
                request
            );
        }
        else
        {
            _logger.LogInformation(
                "Ejecutada solicitud: {Name} en {ElapsedMilliseconds} ms",
                requestName,
                elapsedMilliseconds
            );
        }

        return response;
    }
}
