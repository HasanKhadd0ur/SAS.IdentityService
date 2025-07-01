using SAS.IdentityService.Infrastructure.Services.Tokens;

namespace SAS.IdentityService.Tests.IntegrationTests.Config
{
    public static class TestJwtSettings
        {
            public static JwtSetting Get()
            {
                return new JwtSetting
                {
                    Secret = "THIS_IS_A_SUPER_SECRET_KEY_1234567890",
                    Issuer = "TestIssuer",
                    Audience = "TestAudience",
                    ExpireMinutes = 60
                };
            }
        }
    

}
