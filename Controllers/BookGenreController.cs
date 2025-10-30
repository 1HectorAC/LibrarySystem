
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("bookGenre/[action]")]
[ApiController]
public class BookGenreController : ControllerBase
{
    private readonly LibraryDbContext _context;
    public BookGenreController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookGenre>>> BookGenres()
    {
        var bookGenres = await _context.BookGenres.ToListAsync();
        return Ok(bookGenres);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookGenre>> BookGenre(int id)
    {
        var bookGenre = await _context.BookGenres.FindAsync(id);
        if (bookGenre is null)
            return NotFound(new { Message = $"BookGenre with id {id} not found." });
        return Ok(bookGenre);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBookGenre(int id)
    {
        var bookGenre = await _context.BookGenres.FindAsync(id);

        if (bookGenre is null)
            return NotFound(new { Message = $"BookGenre with id {id} not found." });

        _context.Remove(bookGenre);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<BookGenre>> UpdateBookGenre(int id, [FromBody] BookGenre updatedBookGenre)
    {
        var bookGenre = await _context.BookGenres.FindAsync(id);

        if (bookGenre is null)
            return NotFound(new { Message = $"BookGenre with id {id} not found." });

        // Validate book exists
        bool bookCheck = _context.Books.Any(i => i.Id == updatedBookGenre.BookId);
        if (!bookCheck)
            return BadRequest(new { Message = "BookId does not exists in db." });

        // Validate genre exists
        bool genreCheck = _context.Genres.Any(i => i.Id == updatedBookGenre.GenreId);
        if (!genreCheck)
            return BadRequest(new { Message = "GenreId does not exist in db." });

        bookGenre.BookId = updatedBookGenre.BookId;
        bookGenre.GenreId = updatedBookGenre.GenreId;
        await _context.SaveChangesAsync();

        return Ok(bookGenre);
    }

    [HttpPost]
    public async Task<ActionResult<BookGenre>> AddBookGenre([FromBody] BookGenre bookGenre)
    {
        _context.Add(bookGenre);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(BookGenre), new {id = bookGenre.Id }, bookGenre);
    }
}