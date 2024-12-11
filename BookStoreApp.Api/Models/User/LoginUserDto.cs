using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Api.Models.User;

public class LoginUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty!;
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty!;
}