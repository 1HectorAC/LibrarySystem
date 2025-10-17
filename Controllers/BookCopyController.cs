
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("bookCopy/[action]")]
[ApiController]
public class BookCopyController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public BookCopyController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookCopy>>> BookCopies()
    {
        var bookCopies = await _context.BookCopies.ToListAsync();
        return Ok(bookCopies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookCopy>> BookCopy(int id)
    {
        var bookCopy = _context.BookCopies.FirstOrDefaultAsync(i => i.Id.Equals(id));
        if (bookCopy is null) return NotFound();
        return Ok(bookCopy);
    }

    [HttpPost]
    public async Task<ActionResult<BookCopy>> BookCopy([FromBody] BookCopy bookCopy)
    {
        _context.BookCopies.Add(bookCopy);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(BookCopy), new {id = bookCopy.Id }, bookCopy);
    }
}