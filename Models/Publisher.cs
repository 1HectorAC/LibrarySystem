
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarySystem.Models;

public class Publisher
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    // Fields to consider  adding: address, phone

    [JsonIgnore]
    public ICollection<Book> Books { get; set; } = new List<Book>();
}