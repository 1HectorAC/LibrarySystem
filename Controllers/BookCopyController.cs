
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

// Only allow admin and employee roles to access these endpoints
[Authorize(Roles = "admin,employee")]
[Route("api/book-copies")]
[ApiController]
public class BookCopyController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public BookCopyController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookCopy>>> GetBookCopies()
    {
        var bookCopies = await _context.BookCopies.AsNoTracking().ToListAsync();
        return Ok(bookCopies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookCopy>> GetBookCopy(int id)
    {
        var bookCopy = await _context.BookCopies.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        if (bookCopy is null) 
            return NotFound();

        return Ok(bookCopy);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBookCopy(int id)
    {
        var bookCopy = await _context.BookCopies.FirstOrDefaultAsync(i => i.Id == id);
        if (bookCopy is null) 
            return NotFound(new { Message = $"BookCopy with id {id} not found." });
        
        _context.BookCopies.Remove(bookCopy);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Author>> UpdateBookCopy(int id, [FromBody] BookCopy updatedBookCopy)
    {
        var bookCopy = await _context.BookCopies.FirstOrDefaultAsync(a => a.Id == id);
        if (bookCopy is null) 
            return NotFound(new { Message = $"BookCopy with id {id} not found." });

        // Validate BookId exists
        var bookCheck = await _context.Books.AnyAsync(i => i.Id == updatedBookCopy.BookId);
        if (!bookCheck)
            return BadRequest(new { Message = "BookId does not exits in db." });

        bookCopy.BookId = updatedBookCopy.BookId;
        bookCopy.Available = updatedBookCopy.Available;

        await _context.SaveChangesAsync();

        return Ok(bookCopy);
    }

    [HttpPost]
    public async Task<ActionResult<BookCopy>> AddBookCopy([FromBody] BookCopy bookCopy)
    {
        _context.BookCopies.Add(bookCopy);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBookCopy), new {id = bookCopy.Id }, bookCopy);
    }
}