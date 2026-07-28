using MediatR;
using UserService.Dtos;

namespace UserService.Features.Authentication.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<AuthResponse>;
