namespace SAS.IdentityService.ApplicationCore.DTOs.Requests;

public class LoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
