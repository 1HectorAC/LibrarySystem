
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarySystem.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Title { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [Required]
    public int PublisherId { get; set; }

    [JsonIgnore]
    public Publisher? Publisher { get; set; }

    [Required]
    [StringLength(100)]
    public required string Isbn { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [JsonIgnore]
    public Author? Author { get; set; }

    [JsonIgnore]
    public ICollection<BookGenre>? BookGenres { get; set; }

}