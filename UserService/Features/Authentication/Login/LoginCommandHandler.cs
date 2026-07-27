using MediatR;
using UserService.Common.Exceptions;
using UserService.Data;
using UserService.Dtos;
using UserService.Security;

namespace UserService.Features.Authentication.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            throw new InvalidCredentialsException();
        }

        var accessToken = _tokenGenerator.GenerateToken(user);
        var refreshToken = await _refreshTokenService.GenerateAndStoreAsync(user.Id, cancellationToken);

        return new AuthResponse(accessToken, refreshToken);
    }
}
