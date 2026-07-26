using MediatR;

namespace UserService.Features.Authentication.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    public Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // JWTs are stateless: there is no server-side session to invalidate here.
        // The client is responsible for discarding the token.
        return Task.FromResult(Unit.Value);
    }
}
