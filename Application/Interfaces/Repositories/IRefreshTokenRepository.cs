using Domain.Entities.Auth;

namespace Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
    : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
}