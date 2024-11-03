using System.Security.Claims;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.Requests;
using VideStore.Shared.Responses;

namespace VideStore.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Result> DeleteAccountAsync(ClaimsPrincipal userClaims);
        Task<Result<string>> ChangePasswordAsync(ChangePasswordRequest model, ClaimsPrincipal userClaims);
        Task<Result<CurrentUserResponse>> GetCurrentUser(ClaimsPrincipal userClaims);
    }
}
