
namespace LibrarySystem.Models;

public class Author
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? About { get; set; }

    ICollection<Book>? Books { get; set; }
}