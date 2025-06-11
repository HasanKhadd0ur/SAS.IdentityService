using Ardalis.Result;
using System;
using System.Threading.Tasks;

namespace SAS.IdentityService.API.Abstraction
{
    public interface IAuthenticationService
    {
        public Task<Result<AuthenticationResult>> Login(string email , string password);
        public Task<Result<AuthenticationResult>> Register(string email, string userName , string password);


    }

}
