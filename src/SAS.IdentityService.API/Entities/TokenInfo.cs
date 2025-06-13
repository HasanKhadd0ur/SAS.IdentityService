using System.ComponentModel.DataAnnotations;

namespace SAS.IdentityService.API.Entities;
public class TokenInfo
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiredAt { get; set; }
}