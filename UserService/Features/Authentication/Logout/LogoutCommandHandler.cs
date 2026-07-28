using MediatR;
using UserService.Security;

namespace UserService.Features.Authentication.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    #region Fields

    private readonly IRefreshTokenService _refreshTokenService;

    #endregion

    #region Constructors

    public LogoutCommandHandler(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    #endregion

    #region Public Methods

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _refreshTokenService.RevokeAsync(request.RefreshToken, cancellationToken);
        return Unit.Value;
    }

    #endregion
}
