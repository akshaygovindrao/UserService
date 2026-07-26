using MediatR;

namespace UserService.Features.Authentication.Logout;

public record LogoutCommand : IRequest;
