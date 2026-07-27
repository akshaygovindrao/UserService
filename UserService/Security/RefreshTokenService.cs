using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace UserService.Security;

public class RefreshTokenService : IRefreshTokenService
{
    private const int TokenSizeBytes = 32;
    private const string KeyPrefix = "refresh-token:";

    private readonly IDistributedCache _cache;
    private readonly RefreshTokenSettings _settings;

    public RefreshTokenService(IDistributedCache cache, IOptions<RefreshTokenSettings> settings)
    {
        _cache = cache;
        _settings = settings.Value;
    }

    public async Task<string> GenerateAndStoreAsync(Guid userId, CancellationToken cancellationToken)
    {
        var rawToken = GenerateRawToken();

        await _cache.SetStringAsync(
            BuildKey(rawToken),
            userId.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_settings.ExpiryDays),
            },
            cancellationToken);

        return rawToken;
    }

    public async Task<Guid?> ValidateAndRotateAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var key = BuildKey(refreshToken);
        var storedUserId = await _cache.GetStringAsync(key, cancellationToken);

        if (storedUserId is null || !Guid.TryParse(storedUserId, out var userId))
        {
            return null;
        }

        await _cache.RemoveAsync(key, cancellationToken);
        return userId;
    }

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return _cache.RemoveAsync(BuildKey(refreshToken), cancellationToken);
    }

    private static string GenerateRawToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(TokenSizeBytes);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string BuildKey(string rawToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return KeyPrefix + Convert.ToHexString(hash);
    }
}
