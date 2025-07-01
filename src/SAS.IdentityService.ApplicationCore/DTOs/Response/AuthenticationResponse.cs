using SAS.IdentityService.ApplicationCore.Entities;

namespace SAS.IdentityService.ApplicationCore.DTOs.Response
{
    public class AuthenticationResponse
    {
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public ICollection<Role> Roles { get; set; }
        public string Token { get; set; }
        public TokenInfo TokenInfo { get; set; }

    }

}
