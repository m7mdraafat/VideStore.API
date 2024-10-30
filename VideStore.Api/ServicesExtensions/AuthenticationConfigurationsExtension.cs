using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VideStore.Domain.ConfigurationsData;

namespace VideStore.Api.ServicesExtensions
{
    public static class AuthenticationConfigurationsExtension
    {

        public static IServiceCollection AddAuthConfigurations(this IServiceCollection services, JwtData jwtData, GoogleData googleData)
        {

            // AddAuthentication(): this method take one argument(Default Scheme)
            // and when we using .AddJwtBearer(): this method can take from you another scheme and options
            // and can take just options and this worked on the default scheme that you have written it in AddAuthentication()
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // we use it for to be don't have to let every end point what is the scheme because it will make every end point work on bearer scheme.
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = jwtData.ValidAudience,
                    ValidateIssuer = true,
                    ValidIssuer = jwtData.ValidIssuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtData.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                };
            })
            // if you need to doing some options on another scheme
            .AddJwtBearer("Bearer2", options =>
            {

            })
            .AddGoogle(options =>
            {
                options.ClientId = googleData.ClientId;
                options.ClientSecret = googleData.ClientSecret;
                options.CallbackPath = "/signin-google";
            });

            return services;
        }
    }
}
