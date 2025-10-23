
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DTO;

public class BookDto
{
    public int Id { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Isbn { get; set; }

    [StringLength(100)]
    public string? AuthorName { get; set; }

    public List<string> Genres { get; set; } = new();

    [StringLength(100)]
    public string? PublisherName { get; set; }
}