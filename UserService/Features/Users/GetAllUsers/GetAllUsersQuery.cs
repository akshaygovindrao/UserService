using MediatR;
using UserService.Dtos;

namespace UserService.Features.Users.GetAllUsers;

public record GetAllUsersQuery(Guid? Cursor, int PageSize) : IRequest<UserPageResponse>;
