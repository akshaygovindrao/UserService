using Microsoft.EntityFrameworkCore;
using UserService.Domain;

namespace UserService.Data;

public class UserRepository : IUserRepository
{
    #region Fields

    private readonly UserServiceDbContext _dbContext;

    #endregion

    #region Constructors

    public UserRepository(UserServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion

    #region Public Methods

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetPageAsync(Guid? cursor, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Users.AsNoTracking();

        if (cursor is { } cursorId)
        {
            query = query.Where(u => u.Id.CompareTo(cursorId) > 0);
        }

        // Fetch one extra row so the handler can tell whether another page follows.
        return await query
            .OrderBy(u => u.Id)
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { id }, cancellationToken);
        if (user is not null)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion
}
