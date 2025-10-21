
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarySystem.Models;

public class BookCopy
{
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }

    [JsonIgnore]
    public Book? Book { get; set; }

    [Required]
    public bool Available { get; set; }

    [JsonIgnore]
    public ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();
}