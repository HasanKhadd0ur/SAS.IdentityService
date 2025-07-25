﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SAS.IdentityService.Infrastructure.Services.Tokens
{
    public sealed class JwtBearerTokenValidationConfiguration
    : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly JwtSetting _jwtSettings ;

        public JwtBearerTokenValidationConfiguration(IOptions<JwtSetting> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public void Configure(string name, JwtBearerOptions options) => Configure(options);

        public void Configure(JwtBearerOptions options)
        {
            var rsaKey = RsaKeyUtils.LoadPrivateKey(_jwtSettings.PrivateKey);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = rsaKey,
            };
        }
    }
}
