using Microsoft.Extensions.Primitives;

namespace UserService.Common.Caching;

public class CacheInvalidator<TEntity> : ICacheInvalidator<TEntity>
{
    private readonly object _lock = new();
    private CancellationTokenSource _resetTokenSource = new();

    public IChangeToken CurrentToken => new CancellationChangeToken(_resetTokenSource.Token);

    public void InvalidateAll()
    {
        lock (_lock)
        {
            var previous = _resetTokenSource;
            _resetTokenSource = new CancellationTokenSource();
            previous.Cancel();
            previous.Dispose();
        }
    }
}
