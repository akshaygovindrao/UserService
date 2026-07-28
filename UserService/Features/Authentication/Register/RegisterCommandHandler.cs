using MediatR;
using UserService.Common.Exceptions;
using UserService.Data;
using UserService.Domain;
using UserService.Dtos;
using UserService.Security;

namespace UserService.Features.Authentication.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    #region Fields

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    #endregion

    #region Constructors

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    #endregion

    #region Public Methods

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new UserAlreadyExistsException(request.Email);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAtUtc = DateTime.UtcNow,
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterResponse("User registered successfully.");
    }

    #endregion
}
