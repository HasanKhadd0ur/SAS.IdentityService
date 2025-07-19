namespace SAS.IdentityService.Infrastructure.Services.Tokens
{
    public class JwtSetting
    {
        public const string Section = "JwtSettings";
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string Audience { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        //public string Secret { get; set; } = null!;
        public int ExpireMinutes { get; set; }
    }

}
