using Users.Domain.Enums;

namespace Users.Application.Auth;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email, UserRole role);
}
