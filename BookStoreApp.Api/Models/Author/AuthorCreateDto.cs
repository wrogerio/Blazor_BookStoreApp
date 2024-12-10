using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Api.Models.Author;

public class AuthorCreateDto
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(500)]
    public string? Bio { get; set; }
}
