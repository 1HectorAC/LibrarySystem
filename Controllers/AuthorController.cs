
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("author/[action]")]
[ApiController]
public class AuthorController: ControllerBase
{
    private readonly LibraryDbContext _context;
    public AuthorController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Author>>> Authors()
    {

        return Ok(await _context.Authors.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Author>> Author(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(i => i.Id.Equals(id));
        if (author == null) return NotFound();
        return Ok(author);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(i => i.Id == id);
        if (author is null) 
            return NotFound(new { Message = $"Author with id {id} not found." });
        
        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Author>> UpdateAuthor(int id, [FromBody] Author updatedAuthor)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
        if (author is null) 
            return NotFound(new { Message = $"Author with id {id} not found." });
        
        author.FirstName = updatedAuthor.FirstName;
        author.LastName = updatedAuthor.LastName;
        author.About = updatedAuthor.About;

        await _context.SaveChangesAsync();

        return Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<Author>> Author([FromBody] Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Author), new {id=author.Id }, author);
    }

}