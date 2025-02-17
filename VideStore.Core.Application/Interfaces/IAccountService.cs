using System.Security.Claims;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs;
using VideStore.Shared.DTOs.Requests;
using VideStore.Shared.DTOs.Requests.Users;
using VideStore.Shared.DTOs.Responses.Users;

namespace VideStore.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Result> DeleteAccountAsync(ClaimsPrincipal userClaims);
        Task<Result<string>> ChangePasswordAsync(ChangePasswordRequest model, ClaimsPrincipal userClaims);
        Task<Result<CurrentUserResponse>> GetCurrentUser(ClaimsPrincipal userClaims);
        Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync();
        Task<Result<string>> RevokeRefreshTokenAsync();
        Task<Result<string>> CreateUserAddress(ClaimsPrincipal userClaims, UserAddressDto userAddressRequest);
        Task<Result<UserAddressDto>> UpdateUserAddress(ClaimsPrincipal userClaims, string addressName, UserAddressDto userAddressRequest);
        Task<Result<string>> DeleteUserAddressAsync(ClaimsPrincipal userClaims, string addressName);
        Task<Result<string>> UpdateUserDataAsync(ClaimsPrincipal userClaims, UpdateUserDto updateUserDto);

        #region User Roles
        Task<Result<string>> AddUserRoleAsync(ClaimsPrincipal userClaims, string roleName);
        Task<Result<string>> RemoveUserRoleAsync(ClaimsPrincipal userClaims, string roleName);
        #endregion
    }
}
