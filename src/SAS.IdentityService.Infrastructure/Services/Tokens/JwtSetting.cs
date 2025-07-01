namespace SAS.IdentityService.Infrastructure.Services.Tokens
{
    public class JwtSetting
    {
        public const string Section = "JwtSettings";

        public string Audience { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Secret { get; set; } = null!;
        public int ExpireMinutes { get; set; }
    }
}
