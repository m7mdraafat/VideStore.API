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
using VideStore.Shared.Responses;

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
            var refreshTokenFromCookie = httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];

            var user = userManager.Users
                .SingleOrDefault(u => u.RefreshTokens!.Any(t => t.Token == refreshTokenFromCookie));

            if (user?.RefreshTokens is null)
                return Result.Failure<AppUserResponse>(new Error(401, "Invalid or inactive refresh token."));

            var refreshToken = user.RefreshTokens
                .Single(t => t.Token == refreshTokenFromCookie);

            if (refreshToken.IsActive is false)
                return Result.Failure<AppUserResponse>(new Error(401, "Invalid or inactive refresh token."));

            refreshToken.RevokedAt = DateTime.UtcNow;

            // generate new Refresh token and add it to user
            var newRefreshToken = await GenerateRefreshTokenAsync();
            user.RefreshTokens.Add(newRefreshToken);

            await userManager.UpdateAsync(user);

            // new JWT Token
            var accessToken = await GenerateAccessTokenAsync(user);

            var userResponse = new AppUserResponse
            {
                Email = user.Email!,
                FirstName = user.DisplayName.Split(' ')[0],
                LastName = user.DisplayName.Split(" ")[1],
                PhoneNumber = user.PhoneNumber!,
                Role = (await userManager.GetRolesAsync(user)).FirstOrDefault(),
                Token = accessToken,
                RefreshTokenExpiration = newRefreshToken.ExpireAt.ToString("dd/MM/yyyy hh:mm tt"),
            };
            await SetRefreshTokenInCookieAsync(newRefreshToken.Token, newRefreshToken.ExpireAt);

            return Result.Success(userResponse);
        }
        #endregion

        #region Revoke Refresh Token
        public async Task<Result> RevokeRefreshTokenAsync()
        {
            var refreshTokenFromCookie = httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens!.Any(t => t.Token == refreshTokenFromCookie));

            var refreshToken = user!.RefreshTokens.Single(t => t.Token == refreshTokenFromCookie);

            if (refreshToken.IsActive is false)
                return Result.Failure(new Error(400, "Invalid or inactive refresh token."));


            refreshToken.RevokedAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            return Result.Success("Refresh token revoked successfully.");
        }
        #endregion

        #region Generate Access Token
        public async Task<string> GenerateAccessTokenAsync(AppUser user)
        {
            var authClaims = new List<Claim>
            {
                new (ClaimTypes.GivenName, user.UserName!),
                new (ClaimTypes.Email, user.Email!)
            };

            var userRoles = await userManager.GetRolesAsync(user);
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            // secretKey
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtData.SecretKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(_jwtData.DurationInMinutes),
                Claims = authClaims.ToDictionary(c => c.Type, c => (object)c.Value),
                Audience = _jwtData.ValidAudience,
                Issuer = _jwtData.ValidIssuer,
                SigningCredentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature),
                // EncryptingCredentials = new EncryptingCredentials(TokenEncryption.RsaKey, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Create token and return encrypted JWT (JWE) 
            return tokenHandler.WriteToken(token);
        }
        #endregion

        #region Generate Refresh Token
        public async Task<RefreshToken> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];

#pragma warning disable SYSLIB0023
            using var generator = new RNGCryptoServiceProvider();
#pragma warning restore SYSLIB0023
            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpireAt = DateTime.UtcNow.AddDays(_jwtData.RefreshTokenExpirationInDays),
                CreatedAt = DateTime.UtcNow,
            };
        }
        #endregion

        #region Append the refresh token to response cookie
        public async Task SetRefreshTokenInCookieAsync(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
            };

            httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
        #endregion


    }
}
