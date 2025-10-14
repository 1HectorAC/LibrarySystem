
namespace LibrarySystem.Models;

public class Checkout
{
    public int Id { get; set; }

    public int BookCopyId { get; set; }

    public BookCopy? BookCopy { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public DateTime CheckoutDate { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(30);

    public DateTime? DateReturned { get; set; }
}