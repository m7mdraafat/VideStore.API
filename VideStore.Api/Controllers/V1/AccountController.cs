using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideStore.Api.Extensions;
using VideStore.Application.Interfaces;
using VideStore.Shared.Requests;
using VideStore.Shared.Requests.Users;
using VideStore.Shared.Responses.Users;

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
    }
}
