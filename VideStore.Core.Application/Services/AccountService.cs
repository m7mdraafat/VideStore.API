
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.Requests;
using VideStore.Shared.Requests.Users;
using VideStore.Shared.Responses.Users;

namespace VideStore.Application.Services
{
    public class AccountService
        (UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            ITokenService tokenService) : IAccountService
    {

        #region Get Current User
        public async Task<Result<CurrentUserResponse>> GetCurrentUser(ClaimsPrincipal userClaims)
        {
            var email = userClaims.FindFirst(ClaimTypes.Email)!.Value;

            var user = await userManager.FindByEmailAsync(email);

            var phoneNumber = user!.PhoneNumber;
            if (user.PhoneNumber == "EMPTY")
                phoneNumber = null;
            var userResponse = new CurrentUserResponse
                (user!.DisplayName.Split(' ')[0], user!.DisplayName.Split(' ')[1], user.Email!, phoneNumber!);

            return Result.Success<CurrentUserResponse>(userResponse);
        }
        #endregion

        #region Delete Account

        public async Task<Result> DeleteAccountAsync(ClaimsPrincipal userCliams)
        {
            var userEmail = userCliams.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Result.Failure<string>(new Error(400, "Email is not found in claims."));

            var user = await userManager.FindByEmailAsync(userEmail);
            if (user is null)
                return Result.Failure<string>(new Error(400, "User not found."));

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded is true) return Result.Success("Account deleted successfully.");
            // Handle case where deletion fails
            var errors = result.Errors.Select(e => e.Description);
            return Result.Failure<string>(new Error(400, $"Account deletion failed: {string.Join(", ", errors)}"));

        }

        #endregion

        #region Change Password
        public async Task<Result<string>> ChangePasswordAsync(ChangePasswordRequest model, ClaimsPrincipal userClaims)
        {
            var userEmail = userClaims!.FindFirstValue(ClaimTypes.Email)!;

            if (await userManager.FindByEmailAsync(userEmail!) is not { } user)
                return Result.Failure<string>(new Error(400, "No account associated with the provided email address was found. Please check the email and try again."));

            if (user.EmailConfirmed is false)
                return Result.Failure<string>(new Error(400, "Please verify your email address before proceeding."));

            if (string.IsNullOrEmpty(model.OldPassword)) return Result.Success<string>("Old password missing.");

            var oldPasswordValid = await userManager.CheckPasswordAsync(user, model.OldPassword);

            if (!oldPasswordValid)
                return Result.Failure<string>(new Error(400, "The old password is incorrect."));

            // Change password using ChangePasswordAsync for profile update
            await unitOfWork.BeginTransactionAsync();

            try
            {
                // For reset password scenario, remove the current password
                var removeCurrentPasswordResult = await userManager.RemovePasswordAsync(user);

                if (!removeCurrentPasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", removeCurrentPasswordResult.Errors.Select(e => e.Description));
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<string>(new Error(400, errors));
                }

                // Add the new password
                var addNewPasswordResult = await userManager.AddPasswordAsync(user, model.NewPassword);

                if (!addNewPasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", addNewPasswordResult.Errors.Select(e => e.Description));
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<string>(new Error(400, errors));
                }

                // Commit the transaction if everything succeeds
                await unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                return Result.Failure<string>(new Error(500, "An error occurred while changing the password. Please try again."));
            }

            return Result.Success<string>("Password changed successfully.");


        }


        #endregion

        #region Create Access Token By RefreshToken
        public async Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync()
        {
            return await tokenService.CreateAccessTokenByRefreshTokenAsync();
        }
        #endregion

        #region Revoke Refresh Token
        public async Task<Result<string>> RevokeRefreshTokenAsync()
        {
            return await tokenService.RevokeRefreshTokenAsync();
        }
        #endregion



    }
}
