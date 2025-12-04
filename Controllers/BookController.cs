
using LibrarySystem.Data;
using LibrarySystem.DTO;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LibrarySystem.Controllers;

[Route("api/books")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly LibraryDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
    public BookController(LibraryDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookDto>>> GetBooks([FromQuery] string? genreName, [FromQuery] string? authorFirstName, [FromQuery] string? authorLastName)
    {
        //Consider adding pagination
        var books = _context.Books
            .Include(i => i.BookGenres).ThenInclude(i => i.Genre)
            .Include(i => i.Author)
            .Include(i => i.Publisher)
            .AsQueryable();

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
            AuthorName = b.Author == null ? "" : b.Author.FirstName + " " + b.Author.LastName,
            PublisherName = b.Publisher == null ? "" : b.Publisher.Name,
            Genres = b.BookGenres.Where(bg => bg.Genre != null).Select(bg => bg.Genre!.Name).ToList(),

        }).ToListAsync();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        var cacheKey = $"Books_{id}";

        if (!_cache.TryGetValue(cacheKey, out Book? book))
        {
            // Include related data for BookDto
            book = await _context.Books
                .Include(i => i.BookGenres).ThenInclude(i => i.Genre)
                .Include(i => i.Author).Include(i => i.Publisher)
                .Include(id => id.BookCopies)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id.Equals(id));

            if (book == null)
                return NotFound(new { Message = $"Book with id {id} not found." });

            _cache.Set(cacheKey, book, _cacheExpiration);
        }

        // Second null book check to make BookDto validation is fuffiled.
        if (book is null)
            return NotFound(new { Message = $"Book with id {id} not found." });

        var result = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Isbn = book.Isbn,
            AuthorName = book.Author == null ? "" : book.Author.FirstName + " " + book.Author.LastName,
            PublisherName = book.Publisher == null ? "" : book.Publisher.Name,
            Genres = book.BookGenres.Where(bg => bg.Genre != null).Select(bg => bg.Genre!.Name).ToList(),
            TotalCopies = book.BookCopies.Count,
            AvailableCopies = book.BookCopies.Count(i => i.Available)
        };

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> UpdateBook(int id, [FromBody] CreateBookDto bookDto)
    {
        var book = await _context.Books
            .Include(b => b.BookGenres)
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound(new { Message = $"Book with Id {id} not found." });

        // Validate Author exists
        if (!await _context.Authors.AnyAsync(a => a.Id == bookDto.AuthorId))
            return BadRequest(new { Message = "AuthorId does not match an Author in database." });

        // Validate Publisher exists
        if (!await _context.Publishers.AnyAsync(p => p.Id == bookDto.PublisherId))
            return BadRequest(new { Message = "PublisherId does not match a Publisher in database." });

        // Validate GenreIds exist
        if (bookDto.GenreIds.Count > 0)
        {
            var genres = await _context.Genres.ToListAsync();
            var genreCheck = bookDto.GenreIds.All(id => genres.Any(g => g.Id == id));
            if (!genreCheck)
                return BadRequest(new { Message = "One or more genreId(s) doesn't exist in database" });
        }

        // Update basic properties
        book.Title = bookDto.Title;
        book.Description = bookDto.Description;
        book.AuthorId = bookDto.AuthorId;
        book.PublisherId = bookDto.PublisherId;
        book.Isbn = bookDto.Isbn;

        // Update genres - remove existing and add new
        _context.BookGenres.RemoveRange(book.BookGenres);
        book.BookGenres = bookDto.GenreIds.Select(genreId => new BookGenre
        {
            BookId = id,
            GenreId = genreId
        }).ToList();

        await _context.SaveChangesAsync();
        _cache.Remove($"Books_{id}");

        // Return updated book in same format as GET
        var result = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Isbn = book.Isbn,
            AuthorName = book.Author == null ? "" : book.Author.FirstName + " " + book.Author.LastName,
            PublisherName = book.Publisher == null ? "" : book.Publisher.Name,
            Genres = book.BookGenres.Where(bg => bg.Genre != null).Select(bg => bg.Genre!.Name).ToList(),
        };

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {

        // Get book by Id and include Checkouts with no DateReturned
        var book = await _context.Books
            .Include(b => b.BookCopies)
            .ThenInclude(bc => bc.Checkouts.Where(c => c.DateReturned == null))
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound(new { Message = $"Book with ID {id} not found." });

        // Check for active checkouts
        if (book.BookCopies.Any(bc => bc.Checkouts.Count != 0))
        {
            return BadRequest(new { Message = "Cannot delete book with active checkouts." });
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        _cache.Remove($"Books_{id}");

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> AddBook([FromBody] CreateBookDto book)
    {
        // Check if Author exits
        bool authorCheck = _context.Authors.Any(a => a.Id.Equals(book.AuthorId));
        if (!authorCheck)
            return BadRequest(new { Message = "AuthorId does not match an Author in database." });

        //Check if Publisher exits
        bool publisherCheck = _context.Publishers.Any(p => p.Id.Equals(book.PublisherId));
        if (!publisherCheck)
            return BadRequest(new { Message = "PublisherId does not match a Publisher in database." });

        //Check if GenreIds exist
        var genreIds = book.GenreIds;
        if (genreIds.Count > 0)
        {
            var genres = _context.Genres.ToList();
            var genreCheck = genreIds.All(i => genres.Any(j => j.Id == i));
            if (!genreCheck)
                return BadRequest(new { Message = "one or more genreId(s) doesn't exist in database" });
        }

        Book result = new Book
        {
            Title = book.Title,
            Description = book.Description,
            AuthorId = book.AuthorId,
            PublisherId = book.PublisherId,
            Isbn = book.Isbn,
            BookGenres = genreIds.Select(id => new BookGenre { GenreId = id }).ToList()
        };

        _context.Books.Add(result);
        await _context.SaveChangesAsync();

        // Consider adding format to result, Maybe use DTO.

        return CreatedAtAction(nameof(Book), new { id = result.Id }, result);
    }
}