using Microsoft.Extensions.Caching.Memory;
using UserService.Common.Caching;
using UserService.Domain;

namespace UserService.Data.Caching;

// Decorates IUserRepository with an in-memory read cache. Any write (add/update/delete)
// invalidates every cached entry via ICacheInvalidator<User>, since the cheap alternative -
// individually keyed invalidation across GetById/GetByEmail/GetPage(cursor,size) combinations -
// would need to track every key ever issued.
public class CachedUserRepository : IUserRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IUserRepository _inner;
    private readonly IMemoryCache _cache;
    private readonly ICacheInvalidator<User> _invalidator;

    public CachedUserRepository(IUserRepository inner, IMemoryCache cache, ICacheInvalidator<User> invalidator)
    {
        _inner = inner;
        _cache = cache;
        _invalidator = invalidator;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var key = $"user:email:{email}";
        if (_cache.TryGetValue(key, out User? cached))
        {
            return cached;
        }

        var user = await _inner.GetByEmailAsync(email, cancellationToken);
        if (user is not null)
        {
            Set(key, user);
        }

        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = $"user:id:{id}";
        if (_cache.TryGetValue(key, out User? cached))
        {
            return cached;
        }

        var user = await _inner.GetByIdAsync(id, cancellationToken);
        if (user is not null)
        {
            Set(key, user);
        }

        return user;
    }

    public async Task<IReadOnlyList<User>> GetPageAsync(Guid? cursor, int pageSize, CancellationToken cancellationToken)
    {
        var key = $"user:page:{cursor}:{pageSize}";
        if (_cache.TryGetValue(key, out IReadOnlyList<User>? cached) && cached is not null)
        {
            return cached;
        }

        var page = await _inner.GetPageAsync(cursor, pageSize, cancellationToken);
        Set(key, page);
        return page;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _inner.AddAsync(user, cancellationToken);
        _invalidator.InvalidateAll();
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        await _inner.UpdateAsync(user, cancellationToken);
        _invalidator.InvalidateAll();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _inner.DeleteAsync(id, cancellationToken);
        _invalidator.InvalidateAll();
    }

    private void Set<T>(string key, T value)
    {
        using var entry = _cache.CreateEntry(key);
        entry.Value = value;
        entry.AbsoluteExpirationRelativeToNow = CacheDuration;
        entry.AddExpirationToken(_invalidator.CurrentToken);
    }
}
