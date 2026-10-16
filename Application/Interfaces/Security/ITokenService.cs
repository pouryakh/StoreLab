using Domain.Entities;

namespace Application.Interfaces.Security;

public interface ITokenService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);

    DateTime GetRefreshTokenExpiration();
}