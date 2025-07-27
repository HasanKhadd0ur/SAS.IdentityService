using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SAS.IdentityService.API.Services;
using SAS.IdentityService.ApplicationCore.Contracts.Authentication;
using SAS.IdentityService.ApplicationCore.Contracts.Tokens;
using SAS.IdentityService.ApplicationCore.Contracts.Users;
using SAS.IdentityService.Infrastructure.Services.Authentication;
using SAS.IdentityService.Infrastructure.Services.Tokens;
using SAS.IdentityService.Infrastructure.Services.Users;
using System.Text;

namespace SAS.EventsService.Infrastructure.Services.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureSevices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddBackgroundServices(configuration)
                .AddCronJobs()
                .AddServices(configuration);

            return services;
        }

        #region Add Servcies 
        private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Load JWT settings
            var jwtSettingsSection = configuration.GetSection("JwtSettings");

            services.Configure<JwtSetting>(jwtSettingsSection);
            var jwtSettings = jwtSettingsSection.Get<JwtSetting>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            services.AddAuthentication(options =>
            {

                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                

                options.LoginPath = "/api/auth/login";
                options.AccessDeniedPath = "/api/auth/access-denied";
                options.SlidingExpiration = true;
            })
            .AddGoogle("Google", options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"];
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/api/auth/signin-google";
                options.SaveTokens = true;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;

                var rsaKey = RsaKeyUtils.LoadPrivateKey(jwtSettings.PrivateKey);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = rsaKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Add services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }

        #endregion Add Servcies 

        #region Background jobs 
        private static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        #endregion Background jobs 

        #region Cron Jobs
        private static IServiceCollection AddCronJobs(this IServiceCollection services)
        {

            return services;

        }
        #endregion
    }
}
