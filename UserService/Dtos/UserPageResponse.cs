namespace UserService.Dtos;

public record UserPageResponse(IReadOnlyList<UserProfileResponse> Items, Guid? NextCursor);
