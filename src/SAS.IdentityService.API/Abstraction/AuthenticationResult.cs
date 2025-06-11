using SAS.IdentityService.API.Entities;

namespace SAS.IdentityService.API.Abstraction
{
    public class AuthenticationResult
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public ICollection<Role> Roles { get; set; }
        public string Token { get; set; }
        public TokenInfo TokenInfo { get; set; }

    }

}
