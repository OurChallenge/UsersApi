using FIAP_Cloud_Games.Domain.Entities;
using FIAP_Cloud_Games.Domain.Enums;

namespace FIAP_Cloud_Games.Application.Auth;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email, UserRole role);
}