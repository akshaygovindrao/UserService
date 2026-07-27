namespace UserService.Security;

public interface IRefreshTokenService
{
    Task<string> GenerateAndStoreAsync(Guid userId, CancellationToken cancellationToken);
    Task<Guid?> ValidateAndRotateAsync(string refreshToken, CancellationToken cancellationToken);
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken);
}
