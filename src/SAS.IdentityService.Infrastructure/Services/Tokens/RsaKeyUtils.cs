using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace SAS.IdentityService.Infrastructure.Services.Tokens
{
    public static class RsaKeyUtils
    {
        public static RsaSecurityKey LoadPrivateKey(string base64Key)
        {
            var rsa = RSA.Create();
            byte[] privateKeyBytes = Convert.FromBase64String(base64Key);
            rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            return new RsaSecurityKey(rsa);
        }
    }

}
