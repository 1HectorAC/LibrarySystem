
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarySystem.Models;

public class Genre
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [JsonIgnore]
    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}