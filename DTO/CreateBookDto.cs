
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DTO;

public class CreateBookDto
{
    [Required]
    [StringLength(100)]
    public required string Title { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [StringLength(100)]
    public required string Isbn { get; set; }

    [Required]
    public required int AuthorId { get; set; }

    public List<int> GenreIds { get; set; } = new();

    [Required]
    public int PublisherId { get; set; }
}