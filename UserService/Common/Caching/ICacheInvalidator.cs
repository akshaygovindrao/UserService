using Microsoft.Extensions.Primitives;

namespace UserService.Common.Caching;

// One instance per closed generic TEntity (via open-generic DI registration), so invalidating
// one entity type's cache never touches another's.
public interface ICacheInvalidator<TEntity>
{
    IChangeToken CurrentToken { get; }
    void InvalidateAll();
}
