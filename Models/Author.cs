
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarySystem.Models;

public class Author
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(200)]
    public string? About { get; set; }

    [JsonIgnore]
    public ICollection<Book>? Books { get; set; }
}