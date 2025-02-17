using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VideStore.Application.Interfaces;
using VideStore.Domain.ConfigurationsData;
using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Requests.Users;
using VideStore.Shared.DTOs.Responses.Users;

namespace VideStore.Infrastructure.ExternalServices
{
    public class GoogleAuthService
        (UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenProviderService,
            IOptions<GoogleData> googleDataOptions) : IGoogleService
    {
        private readonly GoogleData _googleData = googleDataOptions.Value;

        public async Task<Result<AppUserResponse>> GoogleSignInAsync(GoogleRequest credentials)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _googleData.ClientId }
                };

                try
                {
                    // Validate the Google token and get payload
                    var payload = await GoogleJsonWebSignature.ValidateAsync(credentials.IdToken, settings);

                    if (string.IsNullOrEmpty(payload.Email))
                        return Result.Failure<AppUserResponse>(new Error(400, "Invalid payload: Email is missing"));

                    // Check if the user already exists in the system
                    var user = await userManager.FindByEmailAsync(payload.Email);

                    if (user is null)
                    {
                        // If user doesn't exist, register a new one
                        user = new AppUser
                        {
                            DisplayName = payload.Name,
                            Email = payload.Email,
                            UserName = payload.Email.Split('@')[0],
                            PhoneNumber = "EMPTY",
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = false,
                        };

                        // Create the new user
                        var createResult = await userManager.CreateAsync(user);
                        if (!createResult.Succeeded)
                        {
                            return Result.Failure<AppUserResponse>(new Error(500, "Failed to create user"));
                        }

                        if (!await roleManager.RoleExistsAsync("user"))
                        {
                            await roleManager.CreateAsync(new IdentityRole("user"));
                        }
                        await userManager.AddToRoleAsync(user, "user");
                    }

                    // If user exists, continue with login
                    user.EmailConfirmed = true;  // Ensure their email is confirmed since Google verified it

                    // Update user info in the database
                    await userManager.UpdateAsync(user);

                    // Generate the access token for the user
                    var token = await tokenProviderService.GenerateAccessTokenAsync(user);

                    // Handle refresh token
                    RefreshToken refreshToken;
                    if (user.RefreshTokens.Any(t => t.IsActive))
                    {
                        refreshToken = user.RefreshTokens.First(t => t.IsActive);
                    }
                    else
                    {
                        refreshToken = await tokenProviderService.GenerateRefreshTokenAsync();
                        user.RefreshTokens ??= new List<RefreshToken>();
                        user.RefreshTokens.Add(refreshToken);

                        await userManager.UpdateAsync(user);
                    }

                    // Prepare response for the user
                    var userResponse = new AppUserResponse
                    {
                        FirstName = user.DisplayName?.Split(' ')[0] ?? string.Empty,
                        LastName = user.DisplayName?.Split(' ').Skip(1).FirstOrDefault() ?? string.Empty,
                        Email = user.Email!,
                        Token = token,
                        RefreshTokenExpiration = refreshToken.ExpireAt.ToString("dd/MM/yyyy hh:mm tt"),
                        Roles = new List<string>() { "user"},
                        IsVerified = true,

                    };

                    // Set the refresh token in a cookie for the user
                    await tokenProviderService.SetRefreshTokenInCookieAsync(refreshToken.Token, refreshToken.ExpireAt);

                    return Result.Success<AppUserResponse>(userResponse);
                }
                catch (InvalidJwtException ex)
                {
                    return Result.Failure<AppUserResponse>(new Error(400, "Invalid JWT: " + ex.Message));
                }
                catch (Exception ex)
                {
                    return Result.Failure<AppUserResponse>(new Error(500, "An error occurred: " + ex.Message));
                }

            }
            catch (Exception ex)
            {
                return Result.Failure<AppUserResponse>(new Error(500, ex.Message));
            }


        }
    }
}
