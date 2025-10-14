
namespace LibrarySystem.Models;

public class BookCopy
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public Book? Book { get; set; }

    public bool Available { get; set; }
}