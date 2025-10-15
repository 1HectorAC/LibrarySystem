
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarySystem.Models;

public class Checkout
{
    public int Id { get; set; }

    [Required]
    public int BookCopyId { get; set; }

    [JsonIgnore]
    public BookCopy? BookCopy { get; set; }

    [Required]
    public int UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    [Required]
    public DateTime CheckoutDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(30);

    public DateTime? DateReturned { get; set; }
}