using UserService.Domain;

namespace UserService.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
