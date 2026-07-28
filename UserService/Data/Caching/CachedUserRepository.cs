using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Domain;

namespace UserService.Data.Caching;

// Decorates IUserRepository with a Redis-backed read cache shared across every instance.
// Cache keys embed a version number stored in Redis; any write bumps the version so every
// existing key becomes unreachable at once instead of being deleted individually - avoids
// tracking every key ever issued across GetById/GetByEmail/GetPage(cursor,size) combinations.
// Orphaned entries simply expire via their own TTL.
public class CachedUserRepository : IUserRepository
{
    #region Fields

    private const string VersionKey = "cache-version:user";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IUserRepository _inner;
    private readonly IDistributedCache _cache;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    #endregion

    #region Constructors

    public CachedUserRepository(
        IUserRepository inner,
        IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    #endregion

    #region Private Methods

    private async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<Task<T?>> factory,
        CancellationToken ct)
        where T : class
    {
        var cacheKey = await BuildCacheKeyAsync(key, ct);

        var json = await _cache.GetStringAsync(cacheKey, ct);

        if (json is not null)
            return JsonSerializer.Deserialize<T>(json, JsonOptions);

        var value = await factory();

        if (value is null)
            return null;

        json = JsonSerializer.Serialize(value, JsonOptions);

        await _cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            },
            ct);

        return value;
    }

    private async Task<string> BuildCacheKeyAsync(string key, CancellationToken ct)
    {
        var version = await GetVersionAsync(ct);
        return $"{key}:v{version}";
    }

    private async Task<string> GetVersionAsync(CancellationToken ct)
    {
        var version = await _cache.GetStringAsync(VersionKey, ct);

        if (!string.IsNullOrWhiteSpace(version))
            return version;

        version = Guid.NewGuid().ToString("N");

        await _cache.SetStringAsync(VersionKey, version, ct);

        return version;
    }

    private Task InvalidateAsync(CancellationToken ct) =>
        _cache.SetStringAsync(
            VersionKey,
            Guid.NewGuid().ToString("N"),
            ct);

    #endregion

    #region Public Methods

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        GetOrCreateAsync(
            $"user:id:{id}",
            () => _inner.GetByIdAsync(id, ct),
            ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        GetOrCreateAsync(
            $"user:email:{email}",
            () => _inner.GetByEmailAsync(email, ct),
            ct);

    public async Task<IReadOnlyList<User>> GetPageAsync(Guid? cursor, int pageSize, CancellationToken ct)
    {
        var page = await GetOrCreateAsync(
            $"user:page:{cursor}:{pageSize}",
            async () => (await _inner.GetPageAsync(cursor, pageSize, ct)).ToList(),
            ct);
        return page ?? [];
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _inner.AddAsync(user, ct);
        await InvalidateAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        await _inner.UpdateAsync(user, ct);
        await InvalidateAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await _inner.DeleteAsync(id, ct);
        await InvalidateAsync(ct);
    }

    #endregion
}
