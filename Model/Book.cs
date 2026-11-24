
using System.ComponentModel.DataAnnotations;

public record Book(
    [Required]
    [RegularExpression(@"^\d{13}$")]
    string Isbn,
    [Required] [StringLength(200)] string Title,
    [Required] [StringLength(100)] string Author,
    [StringLength(1000)] string ShortDescription,
    [Range(1, 100000)] int PageCount,
    DateOnly ReleaseDate);