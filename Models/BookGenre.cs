
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarySystem.Models;

public class BookGenre
{
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }

    [JsonIgnore]
    public Book? Book { get; set; }

    [Required]
    public int GenreId { get; set; }

    [JsonIgnore]
    public Genre? Genre { get; set; }
}