using System.Security.Claims;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.Requests;
using VideStore.Shared.Responses;

namespace VideStore.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AppUserResponse>> LoginAsync(LoginRequest model);
        Task<Result<AppUserResponse>> RegisterAsync(RegisterRequest model);
        Task<Result<AppUserResponse>> GoogleSignInAsync(GoogleRequest credentials);
        Task<Result<string>> SendEmailVerificationCodeAsync(ClaimsPrincipal userClaims);
        Task<Result> VerifyRegisterCodeAsync(CodeVerificationRequest model, ClaimsPrincipal userClaims);
        Task<Result<string>> SendResetPasswordEmailAsync(ResetPasswordEmailRequest model);
        Task<Result> VerifyResetPasswordCodeAsync(VerifyForgetPasswordRequest model);
        Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest model);

    }
}