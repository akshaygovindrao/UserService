using MediatR;
using UserService.Data;
using UserService.Dtos;

namespace UserService.Features.Users.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, UserPageResponse>
{
    #region Fields

    private readonly IUserRepository _userRepository;

    #endregion

    #region Constructors

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    #endregion

    #region Public Methods

    public async Task<UserPageResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetPageAsync(request.Cursor, request.PageSize, cancellationToken);

        var hasMore = users.Count > request.PageSize;
        var page = hasMore ? users.Take(request.PageSize).ToList() : users;
        var nextCursor = hasMore ? page[^1].Id : (Guid?)null;

        return new UserPageResponse(
            page.Select(u => new UserProfileResponse(u.Id, u.Email)).ToList(),
            nextCursor);
    }

    #endregion
}
