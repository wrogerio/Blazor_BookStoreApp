using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Api.Models.User;

public class UserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty!;

    [StringLength(50)]
    public string Apelido { get; set; } = string.Empty!;

    [Required]
    [StringLength(10)]
    public string Role { get; set; } = string.Empty!;
}
