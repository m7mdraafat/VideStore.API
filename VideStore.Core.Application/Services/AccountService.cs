using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoMapper;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.DTOs.Requests;
using VideStore.Shared.DTOs.Requests.Users;
using VideStore.Shared.DTOs.Responses.Users;
namespace VideStore.Application.Services
{
    public class AccountService
        (UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            ITokenService tokenService, 
            IMapper mapper) : IAccountService
    {

        #region Get Current User
        public async Task<Result<CurrentUserResponse>> GetCurrentUser(ClaimsPrincipal userClaims)
        {
            var email = userClaims.FindFirst(ClaimTypes.Email)!.Value;

            var user = await userManager.Users.Include(u => u.UserAddresses)
                .SingleOrDefaultAsync(u => u.Email == email);

            var userAddresses = mapper.Map<List<UserAddressDto>>(user!.UserAddresses);
            var phoneNumber = user!.PhoneNumber;
            if (string.Equals(user.PhoneNumber, "EMPTY", comparisonType: StringComparison.Ordinal))
                phoneNumber = null;
            var userResponse = new CurrentUserResponse
                (user.DisplayName.Split(' ')[0], user!.DisplayName.Split(' ')[1], user.Email!, phoneNumber!,
                    userAddresses);

            return Result.Success(userResponse);
        }
        #endregion

        #region Delete Account

        public async Task<Result> DeleteAccountAsync(ClaimsPrincipal userCliams)
        {
            var userEmail = userCliams.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Result.Failure<string>(new(400, "Email is not found in claims."));

            var user = await userManager.FindByEmailAsync(userEmail);
            if (user is null)
                return Result.Failure<string>(new(400, "User not found."));

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded is true) return Result.Success("Account deleted successfully.");
            // Handle case where deletion fails
            var errors = result.Errors.Select(e => e.Description);
            return Result.Failure<string>(new(400, $"Account deletion failed: {string.Join(", ", errors)}"));

        }

        #endregion

        #region Change Password
        public async Task<Result<string>> ChangePasswordAsync(ChangePasswordRequest model, ClaimsPrincipal userClaims)
        {
            var userEmail = userClaims!.FindFirstValue(ClaimTypes.Email)!;

            if (await userManager.FindByEmailAsync(userEmail!) is not { } user)
                return Result.Failure<string>(new(400, "No account associated with the provided email address was found. Please check the email and try again."));

            if (user.EmailConfirmed is false)
                return Result.Failure<string>(new(400, "Please verify your email address before proceeding."));

            if (string.IsNullOrEmpty(model.OldPassword)) return Result.Success<string>("Old password missing.");

            var oldPasswordValid = await userManager.CheckPasswordAsync(user, model.OldPassword);

            if (!oldPasswordValid)
                return Result.Failure<string>(new(400, "The old password is incorrect."));

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
                    return Result.Failure<string>(new(400, errors));
                }

                // Add the new password
                var addNewPasswordResult = await userManager.AddPasswordAsync(user, model.NewPassword);

                if (!addNewPasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", addNewPasswordResult.Errors.Select(e => e.Description));
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure<string>(new(400, errors));
                }

                // Commit the transaction if everything succeeds
                await unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                return Result.Failure<string>(new(500, "An error occurred while changing the password. Please try again."));
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

        
        public async Task<Result<string>> CreateUserAddress(ClaimsPrincipal userClaims, UserAddressDto userAddressRequest)
        {
            var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.Users.Include(u => u.UserAddresses).SingleOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return Result.Failure<string>(new Error(404,
                    $"The user with email {userEmail} not registered in the system."));
            }

            userAddressRequest.AddressName = userAddressRequest.AddressName.Replace(' ', '-');
            var userAddress =
                user.UserAddresses.SingleOrDefault(ua => ua.AddressName == userAddressRequest.AddressName);
            if (userAddress != null)
                return Result.Failure<string>(new Error(400, "Address name must be unique."));

            userAddress = mapper.Map<UserAddress>(userAddressRequest);
            userAddress.AppUserId = user.Id;
            user.UserAddresses.Add(userAddress);

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Result.Success<string>("User address added successfully.");

            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<string>(new Error(500, errors));

        }

        public async Task<Result<UserAddressDto>> UpdateUserAddress(ClaimsPrincipal userClaims, string addressName ,UserAddressDto userAddressRequest)
        {
            var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.Users.Include(u => u.UserAddresses)
                .SingleOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Result.Failure<UserAddressDto>(new Error(404,
                    $"The user with email {userEmail} not registered in the system."));

            userAddressRequest.AddressName= userAddressRequest.AddressName.Replace(' ', '-');
            addressName = addressName.Replace(' ', '-');
            var addressToUpdate =
                user.UserAddresses.SingleOrDefault(ua => ua.AddressName == addressName);

            if (addressToUpdate == null)
                return Result.Failure<UserAddressDto>(new Error(400,
                    $"The address with name {addressName} was found for this user."));

            addressToUpdate!.AddressName = userAddressRequest.AddressName;
            addressToUpdate.City = userAddressRequest.City;
            addressToUpdate.AddressLine = userAddressRequest.AddressLine;
            addressToUpdate.PostalCode = userAddressRequest.PostalCode;
            addressToUpdate.Governorate = userAddressRequest.Governorate;

            // Save changes
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Result.Failure<UserAddressDto>(new Error(500, "Failed to update the address."));

            var updatedAddressDto = mapper.Map<UserAddressDto>(addressToUpdate);

            return Result.Success(updatedAddressDto);
        }

        public async Task<Result<string>> DeleteUserAddressAsync(ClaimsPrincipal userClaims, string addressName)
        {
            var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.Users.Include(u => u.UserAddresses).FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Result.Failure<string>(new Error(404,
                    $"The user with email {userEmail} not registered in the system."));

            addressName = addressName.Replace(' ', '-');
            var addressToRemove = user.UserAddresses.FirstOrDefault(u => u.AddressName == addressName);
            if(addressToRemove == null)
                return Result.Failure<string>(new Error(404,
                    $"The address name {addressName} not found."));

            user.UserAddresses.Remove(addressToRemove!);

            var result = await userManager.UpdateAsync(user);

            return !result.Succeeded ? Result.Failure<string>(new(500, "Failed to deleted the address.")) : Result.Success<string>("address deleted successfully");
        }

        public async Task<Result<string>> UpdateUserDataAsync(ClaimsPrincipal userClaims, UpdateUserDto updateUserDto)
        {
            var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(userEmail!);
            if (user == null)
                return Result.Failure<string>(new(404, $"user with email {userEmail} not found!"));

            user.DisplayName = updateUserDto.FirstName + " " +  updateUserDto.LastName;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.PhoneNumberConfirmed = false;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Result.Success<string>("User updated successfully.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));

            return Result.Failure<string>(new(500, errors));
        }

    }
}
