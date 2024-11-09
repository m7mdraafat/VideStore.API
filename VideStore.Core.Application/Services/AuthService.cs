using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using VideStore.Application.DTOs;
using VideStore.Application.Interfaces;
using VideStore.Domain.ConfigurationsData;
using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.Specifications;
using VideStore.Shared.Specifications.IdentityCodesSpecifications;
using VideStore.Shared.DTOs.Requests.Users;
using VideStore.Shared.DTOs.Responses.Users;

namespace VideStore.Application.Services
{
    public class AuthService
            (UserManager<AppUser> userManager,
            ITokenService tokenProviderService, 
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager,
            IOptions<GoogleData> googleOptions,
            IGoogleService googleService,
            IUnitOfWork unitOfWork, 
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        private readonly GoogleData _googleData = googleOptions.Value;

        #region Register
        public async Task<Result<AppUserResponse>> RegisterAsync(RegisterRequest model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is { EmailConfirmed: true }) // pattern match
            {
                return Result.Failure<AppUserResponse>(new Error(400, "The email address you entered is already taken, please try a different one."));
            }


            var newUser = new AppUser
            {
                DisplayName = model.FirstName + " " + model.LastName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
            };

            var result = await userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure<AppUserResponse>(new Error(400, errors));
            }

            // Get token 
            var token = await tokenProviderService.GenerateAccessTokenAsync(newUser);
            // Get new refresh token 
            var refreshToken = await tokenProviderService.GenerateRefreshTokenAsync();

            // Generate user response
            var userResponse = new AppUserResponse
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Role = "user",
                Token = token,
                RefreshTokenExpiration = refreshToken.ExpireAt.ToString("MM/dd/yyyy hh:mm tt"),
                PhoneNumber = model.PhoneNumber,
                IsVerified = false
            };

            if (!await roleManager.RoleExistsAsync("user"))
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }
            await userManager.AddToRoleAsync(newUser, "user");

            await userManager.UpdateAsync(newUser);

            // Set Refresh token in cookie
            await tokenProviderService.SetRefreshTokenInCookieAsync(refreshToken.Token, refreshToken.ExpireAt);

            return Result.Success<AppUserResponse>(userResponse);

        }

        #endregion

        #region Login
        public async Task<Result<AppUserResponse>> LoginAsync(LoginRequest model)
        {
            var user = await userManager.Users
                                         .Include(x => x.RefreshTokens)
                                         .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user is null)
            {
                return Result.Failure<AppUserResponse>(new Error(400, "The email or password you entered is incorrect. Check your credentials and try again."));
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                return Result.Failure<AppUserResponse>(new Error(400, "The email or password you entered is incorrect. Check your credentials and try again."));
            }

            var token = await tokenProviderService.GenerateAccessTokenAsync(user);

            RefreshToken refreshToken;

            // Check if RefreshTokens is not null and has active tokens
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                refreshToken = user.RefreshTokens.First(t => t.IsActive);
            }
            else
            {
                refreshToken = await tokenProviderService.GenerateRefreshTokenAsync();
                user.RefreshTokens ??= new List<RefreshToken>(); // Ensure RefreshTokens is initialized
                user.RefreshTokens.Add(refreshToken);

                // Update the user with the new refresh token
                await userManager.UpdateAsync(user);
            }

            var refreshTokenExpireAt = refreshToken.ExpireAt.ToString("MM/dd/yyyy hh:mm tt");

            var nameParts = user.DisplayName?.Split(" ") ?? Array.Empty<string>();
            var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var userResponse = new AppUserResponse
            {
                FirstName = firstName,
                LastName = lastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Role = (await userManager.GetRolesAsync(user)).FirstOrDefault(),
                Token = token,
                RefreshTokenExpiration = refreshTokenExpireAt,
                IsVerified = user.EmailConfirmed,

            };

            await tokenProviderService.SetRefreshTokenInCookieAsync(refreshToken.Token, refreshToken.ExpireAt);

            return Result.Success<AppUserResponse>(userResponse);
        }


        #endregion

        public Task<Result<AppUserResponse>> GoogleSignInAsync(GoogleRequest credentials)
        {
            return googleService.GoogleSignInAsync(credentials);
        }

        public async Task<Result<string>> SendEmailVerificationCodeAsync(ClaimsPrincipal userClaims)
        {
            try
            {
                // Retrieve user email from claims
                var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);

                // Check if email claim is found
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Result.Failure<string>(new Error(400, "Email claim not found."));
                }

                // Find user by email
                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {

                    return Result.Failure<string>(new Error(400, "User not found."));
                }

                // Check if email is already confirmed
                if (user.EmailConfirmed)
                {

                    return Result.Failure<string>(new Error(400, "Your email is already confirmed."));
                }

                // Generate the secure code for verification
                var code = GenerateSecureCode();

                // Create email subject and body
                var subject = $"✅ {userEmail!.Split('@')[0]}, Your pin code is {code}. \r\nPlease confirm your email address";
                var body = LoadEmailTemplate(
                    code,
                    userEmail.Split("@")[0],
                    "Email Verification",
                    "Thank you for registering with our store. To complete your registration.",
                "verify your email address."
                );

                EmailResponse emailToSend = new(subject, body, userEmail);

                // Save the verification code to the database
                await unitOfWork.Repository<IdentityCode>().AddAsync(new IdentityCode
                {
                    Code = HashCode(code),
                    IsActive = true,
                    User = user,
                    AppUserId = user.Id,
                    ForRegisterationConfirmed = true,
                });

                var result = await unitOfWork.CompleteAsync();

                if (result == 0)
                    return Result.Failure<string>(new Error(500, $"An error occurred while saving the verification code."));


                BackgroundJob.Enqueue(() => emailService.SendEmailMessage(emailToSend));


                return Result.Success<string>("Email was sent successfully!");
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(new Error(500, $"An error occurred while sending email. {ex.Message}"));
            }
        }

        public async Task<Result<string>> VerifyRegisterCodeAsync(CodeVerificationRequest model, ClaimsPrincipal userClaims)
        {
            try
            {
                var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);

                if (await userManager.FindByEmailAsync(userEmail!) is not { } user)
                    return Result.Failure<string>(new Error(400, "No account found with the provided email address."));

                var specification = new ActiveIdentityCodeSpecification(user.Id, true);
                var identityCode = await unitOfWork.Repository<IdentityCode>().GetEntityAsync(specification);

                if (identityCode is null)
                    return Result.Failure<string>(new Error(400,
                        "The reset code is missing or invalid. Please request a new reset code."));

                var lastCode = identityCode.Code;

                if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
                    return Result.Failure<string>(new Error(400,
                        "The reset code is missing or invalid. Please request a new reset code."));

                if (!identityCode.IsActive || identityCode.CreationTime.AddMinutes(10) < DateTime.UtcNow)
                    return Result.Failure<string>(new Error(400,
                        "The reset code has either expired or is not active. Please request a new code."));

                user.EmailConfirmed = true;
                identityCode.IsActive = false;
                identityCode.User = user;
                identityCode.ActivationTime = DateTime.UtcNow;

                unitOfWork.Repository<IdentityCode>().Update(identityCode);
                var userUpdateResult = await userManager.UpdateAsync(user);
                if (!userUpdateResult.Succeeded)
                {
                    // Log error details here for further diagnostics.
                    return Result.Failure<string>(new Error(500, "An error occurred while updating the user."));
                }

                var result = await unitOfWork.CompleteAsync();

               return Result.Success<string>("Account verified successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception details here
                return Result.Failure<string>(new Error(500, $"An unexpected error occurred while processing your request. {ex.Message}"));
            }
        }

        public async Task<Result<string>> SendResetPasswordEmailAsync(ResetPasswordEmailRequest model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return Result.Failure<string>(new Error(400, "This email does not registered with us."));

            var code = GenerateSecureCode();

            var subject = $"✅ {user.DisplayName}, Reset Your Password - Verification Code: {code}";

            var body = LoadEmailTemplate(
                code,
                user.DisplayName,
            "Reset Password",
                "You have requested to reset your password.",
                "reset your password.");

            EmailResponse emailToSend = new(subject, body, model.Email);

            await unitOfWork.Repository<IdentityCode>().AddAsync(new IdentityCode
            {
                Code = HashCode(code),
                User = user,
                AppUserId = user.Id,
                ForRegisterationConfirmed = false // for reset password
            });

            var result = await unitOfWork.CompleteAsync();
            if (result == 0)
                return Result.Failure<string>(new Error(500, $"An error occurred while saving the verification code."));

            BackgroundJob.Enqueue(() => emailService.SendEmailMessage(emailToSend));

            return Result.Success<string>("If your email is registered with us, a password reset email has been successfully sent.");
        }

        public async Task<Result<string>> VerifyResetPasswordCodeAsync(VerifyForgetPasswordRequest model)
        {
            if (await userManager.FindByEmailAsync(model.Email!) is not { } user)
                return Result.Failure<string>(new Error(400, "No account associated with the provided email address was found. Please check the email and try again."));

            var specification = new ActiveIdentityCodeSpecification(user.Id, false);

            var identityCode = await unitOfWork.Repository<IdentityCode>().GetEntityAsync(specification);

            if (identityCode is null)
                return Result.Failure<string>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

            var lastCode = identityCode!.Code;

            if (!ConstantComparison(lastCode, HashCode(model.VerificationCode)))
                return Result.Failure<string>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

            if (identityCode.CreationTime.AddMinutes(10) < DateTime.UtcNow)
                return Result.Failure<string>(new Error(400, "The reset code is missing or invalid. Please request a new reset code."));

            identityCode.IsActive = false;
            identityCode.User = user;
            identityCode.ActivationTime = DateTime.UtcNow;
            unitOfWork.Repository<IdentityCode>().Update(identityCode);

            var userUpdateResult = await userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                // Log error details here for further diagnostics.
                return Result.Failure<string>(new Error(500, "An error occurred while updating the user."));
            }

            var result = await unitOfWork.CompleteAsync();
            return Result.Success<string>("Reset password code verified successfully.");
        }


        #region Reset password

        public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest model)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not { } user)
                return Result.Failure<string>(new Error(400, "No account associated with the provided email address was found. Please check the email and try again."));

            if (user.EmailConfirmed is false)
                return Result.Failure<string>(new Error(400, "Please verify your email address before proceeding."));

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
                return Result.Failure<string>(new Error(500, "An error occurred while resetting the password. Please try again."));
            }

            return Result.Success<string>("Password changed successfully.");
        }


        #endregion
        public async Task<Result<string>> LogoutAsync(ClaimsPrincipal userClaims)
        {
            // Clear the authentication cookie (used by cookie authentication)
            httpContextAccessor.HttpContext?.Response.Cookies.Delete(".AspNetCore.Cookies");

            // Optionally, clear any custom cookies (e.g., refresh token cookie)
            httpContextAccessor.HttpContext?.Response.Cookies.Delete("refreshToken");

            // Sign out the user
            await signInManager.SignOutAsync();

            var userName = userClaims.FindFirst(ClaimTypes.GivenName);
            return Result.Success<string>($"{userName?.Value} logout successfully.");
        }

        #region Generate Secure Code and hash code
        private static string GenerateSecureCode()
        {
            var randomNumber = new byte[4];

            // Fill the array with random bytes
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            // Convert the byte array to an integer
            var result = BitConverter.ToInt32(randomNumber, 0);
            var positiveResult = Math.Abs(result);

            // Ensure the result is a 6-digit code
            var sixDigitCode = positiveResult % 1000000;

            // Return the code as a zero-padded string
            return sixDigitCode.ToString("D6");
        }

        private static string HashCode(string code)
        {
            var sha256 = SHA256.Create();
            var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(code));
            return BitConverter.ToString(hashedBytes!).Replace("-", "");
        }
        private static bool ConstantComparison(string c1, string c2)
        {
            if (c1.Length != c2.Length)
                return false;

            var result = 0;
            for (int i = 0; i < c1.Length; i++)
            {
                result |= c1[i] ^ c2[i];
            }

            return result == 0;
        }
        #endregion

        #region Load email template 
        private static string LoadEmailTemplate(string code, string userName, string title, string message, string Purpose)
        {
            return $@"

                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Email Verification</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            background-color: #f5f5f5;
                            margin: 0;
                            padding: 0;
                        }}

                        .container {{
                            max-width: 600px;
                            margin: auto;
                            padding: 20px;
                            background-color: #ffffff;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}

                        .header {{
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 10px;
                            text-align: center;
                            border-top-left-radius: 8px;
                            border-top-right-radius: 8px;
                        }}

                        .content {{
                            padding: 20px;
                        }}

                        .code {{
                            font-size: 24px;
                            font-weight: bold;
                            text-align: center;
                            margin-top: 20px;
                            margin-bottom: 30px;
                            color: #007bff;
                        }}

                        .footer {{
                            background-color: #f7f7f7;
                            padding: 10px;
                            text-align: center;
                            border-top: 1px solid #dddddd;
                            font-size: 12px;
                            color: #777777;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h2>{title}</h2>
                        </div>
                        <div class=""content"">
                            <p>Dear {userName},</p>
                            <p>{message}, please use the following verification code:</p>
                            <div class=""code"">{code}</div>
                            <p>This code will expire in 10 minutes. Please use it promptly to {Purpose}.</p>
                            <p>If you did not request this verification, please ignore this email.</p>
                        </div>
                        <div class=""footer"">
                            <p>&copy;{DateTime.UtcNow.Year} VideStore. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>


            ";

        }
        #endregion

    }
}
