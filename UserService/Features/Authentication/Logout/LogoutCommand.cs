using MediatR;

namespace UserService.Features.Authentication.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;
