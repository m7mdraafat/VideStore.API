using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideStore.Api.Extensions;
using VideStore.Application.Interfaces;
using VideStore.Shared.DTOs;
using VideStore.Shared.DTOs.Requests;
using VideStore.Shared.DTOs.Requests.Users;
using VideStore.Shared.DTOs.Responses.Users;

namespace VideStore.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        [Authorize]
        [HttpGet("current-account")]
        public async Task<ActionResult<CurrentUserResponse>> CurrentUser()
        {
            var result = await accountService.GetCurrentUser(User);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult<CurrentUserResponse>> Delete()
        {
            var result = await accountService.DeleteAccountAsync(User);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var result = await accountService.ChangePasswordAsync(request, User);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            var result = await accountService.CreateAccessTokenByRefreshTokenAsync();
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshToken()
        {
            var result = await accountService.RevokeRefreshTokenAsync();
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPost("add-address")]
        [Authorize]
        public async Task<ActionResult> AddAddress(UserAddressDto userAddressDto)
        {
            var result = await accountService.CreateUserAddress(User, userAddressDto);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        [HttpPost("update-address/{addressName}")]
        [Authorize]
        public async Task<ActionResult> UpdateAddress(string addressName, UserAddressDto userAddressDto)
        {
            var result = await accountService.UpdateUserAddress(User, addressName, userAddressDto);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        [HttpDelete("delete-address")]
        [Authorize]
        public async Task<ActionResult> DeleteAddress(string addressName)
        {
            var result = await accountService.DeleteUserAddressAsync(User, addressName);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        [Authorize]
        [HttpPut("edit-user")]
        public async Task<ActionResult> EditUserData(UpdateUserDto updateUserDto)
        {
            var result = await accountService.UpdateUserDataAsync(User, updateUserDto);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }


        [Authorize(Policy = "Admin")]
        [HttpPut("assign-user-role")]
        public async Task<ActionResult> AssignUserRole(string roleName)
        {
            var result = await accountService.AddUserRoleAsync(User, roleName);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("remove-user-role")]
        public async Task<ActionResult> RemoveUserRole(string roleName)
        {
            var result = await accountService.RemoveUserRoleAsync(User, roleName);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }
    }
}
