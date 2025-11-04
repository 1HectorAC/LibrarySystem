
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DTO;

public class BookDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Title { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public required string Isbn { get; set; }

    [Required]
    [StringLength(100)]
    public required string AuthorName { get; set; }

    public List<string> Genres { get; set; } = new();

    [Required]
    [StringLength(100)]
    public required string PublisherName { get; set; }

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }
}