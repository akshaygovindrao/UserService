using MediatR;
using UserService.Dtos;

namespace UserService.Features.Authentication.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
