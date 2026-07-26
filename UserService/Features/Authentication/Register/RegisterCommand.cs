using MediatR;
using UserService.Dtos;

namespace UserService.Features.Authentication.Register;

public record RegisterCommand(string Email, string Password) : IRequest<RegisterResponse>;
