
using LibrarySystem.Data;
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
    public async Task<ActionResult<List<Book>>> Books()
    {
        var books = await _context.Books.ToListAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> Book(int id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(i => i.Id.Equals(id));
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> Book([FromBody] Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Book), new {id = book.Id }, book);
    }
}