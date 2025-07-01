namespace SAS.IdentityService.ApplicationCore.DTOs.Requests;

public class UpdatePasswordRequest
{
    public string CurrentPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}