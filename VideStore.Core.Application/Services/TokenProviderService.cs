using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using VideStore.Application.Interfaces;
using VideStore.Domain.ConfigurationsData;
using VideStore.Domain.Entities.IdentityEntities;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Responses.Users;

namespace VideStore.Application.Services
{
    public class TokenProviderService(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, IOptions<JwtData> jwtDataOptions) : ITokenService
    {
        #region Properties

        private readonly JwtData _jwtData = jwtDataOptions.Value;

        #endregion

        #region Create Access Token By RefreshToken
        public async Task<Result<AppUserResponse>> CreateAccessTokenByRefreshTokenAsync()
        {
            try
            {
                var refreshToken = httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Result.Failure<AppUserResponse>(new Error(401, "Refresh token required"));

                var user = await userManager.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

                if (user == null)
                    return Result.Failure<AppUserResponse>(new Error(404, "User not found"));

                var existingRefreshToken = user.RefreshTokens
                    .FirstOrDefault(t => t.Token == refreshToken);

                if (existingRefreshToken == null || !existingRefreshToken.IsActive)
                    return Result.Failure<AppUserResponse>(new Error(401, "Invalid refresh token"));

                // Revoke current refresh token
                existingRefreshToken.RevokedAt = DateTime.UtcNow;

                // Generate new tokens
                var newRefreshToken = await GenerateRefreshTokenAsync();
                user.RefreshTokens.Add(newRefreshToken);

                var accessToken = await GenerateAccessTokenAsync(user);

                // Update user with new tokens
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return Result.Failure<AppUserResponse>(new Error(500, "Failed to update user tokens"));

                // Set response values
                await SetRefreshTokenInCookieAsync(newRefreshToken.Token, newRefreshToken.ExpireAt);

                return Result.Success(new AppUserResponse
                {
                    Email = user.Email!,
                    FirstName = user.DisplayName.Split(' ')[0],
                    LastName = user.DisplayName.Split(' ')[1],
                    PhoneNumber = user.PhoneNumber!,
                    Roles = (await userManager.GetRolesAsync(user)).ToList(),
                    Token = accessToken,
                    RefreshTokenExpiration = newRefreshToken.ExpireAt.ToString("o") // ISO 8601 format
                });
            }
            catch (Exception ex)
            {
                return Result.Failure<AppUserResponse>(new Error(500, $"Token refresh failed: {ex.Message}"));
            }
        }
        #endregion

        #region Revoke Refresh Token
        public async Task<Result<string>> RevokeRefreshTokenAsync()
        {
            var refreshTokenFromCookie = httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens!.Any(t => t.Token == refreshTokenFromCookie));

            var refreshToken = user!.RefreshTokens.Single(t => t.Token == refreshTokenFromCookie);

            if (refreshToken.IsActive is false)
                return Result.Failure<string>(new Error(400, "Invalid or inactive refresh token."));


            refreshToken.RevokedAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            return Result.Success<string>("Refresh token revoked successfully.");
        }
        #endregion

        #region Generate Access Token
        public async Task<string> GenerateAccessTokenAsync(AppUser user)
        {
            var authClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, user.Id),
                new(JwtRegisteredClaimNames.GivenName, user.UserName!),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await userManager.GetRolesAsync(user);
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtData.SecretKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtData.DurationInMinutes),
                Audience = _jwtData.ValidAudience,
                Issuer = _jwtData.ValidIssuer,
                SigningCredentials = new SigningCredentials(
                    authKey,
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        #endregion

        #region Generate Refresh Token
        public async Task<RefreshToken> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpireAt = DateTime.UtcNow.AddDays(_jwtData.RefreshTokenExpirationInDays),
                CreatedAt = DateTime.UtcNow
            };
        }
        #endregion

        #region Append the refresh token to response cookie
        public async Task SetRefreshTokenInCookieAsync(string refreshToken, DateTime expires)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = expires,
                Secure = true,
                IsEssential = true
            };

            var context = httpContextAccessor.HttpContext;
            if (context == null)
                throw new InvalidOperationException("HTTP context is not available");

            context.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        #endregion

    }
}
