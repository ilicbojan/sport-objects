using MediatR;
using Microsoft.Extensions.Logging;
using Application.Common.Interfaces;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours
{
  public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  {
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public RequestPerformanceBehaviour(
        ILogger<TRequest> logger,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
      _timer = new Stopwatch();

      _logger = logger;
      _currentUserService = currentUserService;
      _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
      _timer.Start();

      var response = await next();

      _timer.Stop();

      var elapsedMilliseconds = _timer.ElapsedMilliseconds;

      if (elapsedMilliseconds > 500)
      {
        var requestName = typeof(TRequest).Name;
        var username = _currentUserService.Username ?? string.Empty;
        var userId = string.Empty;

        if (!string.IsNullOrEmpty(username))
        {
          userId = await _identityService.GetUserIdAsync(username);
        }

        _logger.LogWarning("SportObjects Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
            requestName, elapsedMilliseconds, userId, username, request);
      }

      return response;
    }
  }
}
