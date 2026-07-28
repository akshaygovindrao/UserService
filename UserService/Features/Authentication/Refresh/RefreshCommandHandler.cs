using MediatR;
using UserService.Common.Exceptions;
using UserService.Data;
using UserService.Dtos;
using UserService.Security;

namespace UserService.Features.Authentication.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResponse>
{
    #region Fields

    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    #endregion

    #region Constructors

    public RefreshCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    #endregion

    #region Public Methods

    public async Task<AuthResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var userId = await _refreshTokenService.ValidateAndRotateAsync(request.RefreshToken, cancellationToken);
        if (userId is null)
        {
            throw new InvalidCredentialsException();
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        var accessToken = _tokenGenerator.GenerateToken(user);
        var newRefreshToken = await _refreshTokenService.GenerateAndStoreAsync(user.Id, cancellationToken);

        return new AuthResponse(accessToken, newRefreshToken);
    }

    #endregion
}
