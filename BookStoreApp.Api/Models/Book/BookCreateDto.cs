using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Api.Models.Book;

public class BookCreateDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;
    [Required]
    [Range(1000, 9999)]
    public int? Year { get; set; }

    [Required]
    public string Isbn { get; set; } = null!;

    [Required]
    [StringLength(250, MinimumLength = 10)]
    public string Summary { get; set; } = null!;
    public string Image { get; set; } = null!;

    [Required]
    [Range(0, 5000)]
    public double? Price { get; set; }
}
