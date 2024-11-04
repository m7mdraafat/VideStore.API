using System.Security.Claims;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.Requests;
using VideStore.Shared.Requests.Users;
using VideStore.Shared.Responses.Users;

namespace VideStore.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Result> DeleteAccountAsync(ClaimsPrincipal userClaims);
        Task<Result<string>> ChangePasswordAsync(ChangePasswordRequest model, ClaimsPrincipal userClaims);
        Task<Result<CurrentUserResponse>> GetCurrentUser(ClaimsPrincipal userClaims);
        Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync();
        Task<Result<string>> RevokeRefreshTokenAsync();
    }
}
