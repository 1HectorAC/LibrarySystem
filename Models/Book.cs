
namespace LibrarySystem.Models;

public class Book
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string Publisher { get; set; }

    public required string Isbn { get; set; }

    public int AuthorId { get; set; }

    public Author? Author { get; set; }

}