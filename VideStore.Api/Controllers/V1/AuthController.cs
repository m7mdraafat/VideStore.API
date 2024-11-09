using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideStore.Api.Extensions;
using VideStore.Application.Interfaces;
using VideStore.Application.Services;
using VideStore.Shared.DTOs.Requests.Users;
using VideStore.Shared.DTOs.Responses.Users;

namespace VideStore.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AppUserResponse>> Register(RegisterRequest modelRegisterRequest)
        {

            var result = await authService.RegisterAsync(modelRegisterRequest);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserResponse>> Login(LoginRequest modelLoginRequest)
        {

            var result = await authService.LoginAsync(modelLoginRequest);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPost("google-signin")]
        public async Task<ActionResult<AppUserResponse>> GoogleSignIn(GoogleRequest modelGoogleRequest)
        {

            var result = await authService.GoogleSignInAsync(modelGoogleRequest);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPost("send-register-code")]
        [Authorize]
        public async Task<ActionResult> SendRegisterCode()
        {
            var result = await authService.SendEmailVerificationCodeAsync(User);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }
        [HttpPost("verify-register-code")]
        [Authorize]
        public async Task<ActionResult> VerifyRegisterCode(CodeVerificationRequest request)
        {
            var result = await authService.VerifyRegisterCodeAsync(request, User);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPost("send-reset-password-code")]
        public async Task<ActionResult> SendResetPasswordCode(ResetPasswordEmailRequest request)
        {
            var result = await authService.SendResetPasswordEmailAsync(request);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPost("verify-reset-password-code")]
        public async Task<ActionResult> VerifyResetPasswordCode(VerifyForgetPasswordRequest request)
        {
            var result = await authService.VerifyResetPasswordCodeAsync(request);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPut("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var result = await authService.ResetPasswordAsync(request);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var result = await authService.LogoutAsync(User);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }
    }
}