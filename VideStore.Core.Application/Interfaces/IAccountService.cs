using System.Security.Claims;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Requests.Users;
using VideStore.Shared.DTOs.Responses.Users;
using VideStore.Shared.Requests;

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
