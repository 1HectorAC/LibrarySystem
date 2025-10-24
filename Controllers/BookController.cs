
using LibrarySystem.Data;
using LibrarySystem.DTO;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("book/[action]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly LibraryDbContext _context;
    public BookController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookDto>>> Books([FromQuery] string? genreName, [FromQuery] string? authorFirstName, [FromQuery] string? authorLastName)
    {
        //Consider adding pagination
        var books = _context.Books.Include(i => i.BookGenres).ThenInclude(i => i.Genre).Include(i => i.Author).Include(i => i.Publisher).AsQueryable();
        
        if (genreName is not null)
            books = books.Where(i => i.BookGenres.Any(i => i.Genre != null && i.Genre.Name == genreName));
        
        if (authorFirstName is not null)
            books = books.Where(i => i.Author != null && i.Author.FirstName == authorFirstName);

        if (authorLastName is not null)
            books = books.Where(i => i.Author != null && i.Author.LastName == authorLastName);

        // Use AsNoTracking for read only queries to reduce memory and CPU
        //Note: don't run multiple EF queries in parallel on same LibraryDbContext instance

        var result = await books.AsNoTracking().Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Description = b.Description,
            Isbn = b.Isbn,
            AuthorName = b.Author == null ? null : b.Author.FirstName + " " + b.Author.LastName,
            PublisherName = b.Publisher == null ? null : b.Publisher.Name,
            Genres = b.BookGenres.Where(bg => bg.Genre != null).Select(bg => bg.Genre!.Name).ToList(),

        }).ToListAsync();
        
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> Book(int id)
    {
        var book = await _context.Books.Include(i => i.BookGenres).ThenInclude(i => i.Genre).Include(i => i.Author).Include(i => i.Publisher).FirstOrDefaultAsync(i => i.Id.Equals(id));
        if (book == null) return NotFound();

        var result = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Isbn = book.Isbn,
            AuthorName = book.Author == null ? null : book.Author.FirstName + " " + book.Author.LastName,
            PublisherName = book.Publisher == null ? null : book.Publisher.Name,
            Genres = book.BookGenres.Where(bg => bg.Genre != null).Select(bg => bg.Genre!.Name).ToList(),
        };

        return Ok(result);
    }

    // Add Update endpoint

    // Add Delete endpoint

    [HttpPost]
    public async Task<ActionResult<BookDto>> Book([FromBody] Book book)
    {
        //Consider adding CreateBookDto, which has list of genreIds for adding genres at same time

        // Check if Author exits:
        bool authorCheck = _context.Authors.Any(a => a.Id.Equals(book.AuthorId));
        if (!authorCheck)
            return BadRequest(new { Message = "AuthorId does not match an Author in database." });
        //Check if Publisher exits:
        bool publisherCheck = _context.Publishers.Any(p => p.Id.Equals(book.PublisherId));
        if (!publisherCheck)
            return BadRequest(new {Message = "PublisherId does not match a Publisher in database." });

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Book), new {id = book.Id }, book);
    }
}