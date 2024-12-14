using System.Security.Claims;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Requests.Users;
using VideStore.Shared.DTOs.Responses.Users;

namespace VideStore.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AppUserResponse>> LoginAsync(LoginRequest model);
        Task<Result<AppUserResponse>> RegisterAsync(RegisterRequest model);
        Task<Result<AppUserResponse>> GoogleSignInAsync(GoogleRequest credentials);
        Task<Result<string>> SendEmailVerificationCodeAsync(ClaimsPrincipal userClaims);
        Task<Result<string>> VerifyRegisterCodeAsync(CodeVerificationRequest model, ClaimsPrincipal userClaims);
        Task<Result<string>> SendResetPasswordEmailAsync(ResetPasswordEmailRequest model);
        Task<Result<string>> VerifyResetPasswordCodeAsync(VerifyForgetPasswordRequest model);
        Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest model);
        Task<Result<string>> LogoutAsync(ClaimsPrincipal userClaims);
    }
}