using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.Responses.Users;

namespace VideStore.Application.Interfaces
{
    public interface ITokenService
    {
        Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync();
        Task<Result> RevokeRefreshTokenAsync();
        Task<string> GenerateAccessTokenAsync(AppUser user); // Mark as async if I/O is involved
        Task SetRefreshTokenInCookieAsync(string refreshToken, DateTime expires);
        Task<RefreshToken> GenerateRefreshTokenAsync();
    }
}