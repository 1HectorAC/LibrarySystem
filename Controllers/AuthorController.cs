
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LibrarySystem.Controllers;

[Route("author/[action]")]
[ApiController]
public class AuthorController: ControllerBase
{
    private readonly LibraryDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
    public AuthorController(LibraryDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<List<Author>>> Authors()
    {
        var cacheKey = "Authors";
        if(!_cache.TryGetValue(cacheKey, out List<Author>? authors))
        {
            authors = await _context.Authors.ToListAsync();
            _cache.Set(cacheKey, authors, _cacheExpiration);
        }
        return Ok(authors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Author>> Author(int id)
    {
        var cacheKey = $"Authors_{id}";
        if(!_cache.TryGetValue(cacheKey, out Author? author))
        {
            author = await _context.Authors.FirstOrDefaultAsync(i => i.Id.Equals(id));
            if (author is null)
                return NotFound(new { Message = $"Author with id {id} not found." });
            
            _cache.Set(cacheKey, author, _cacheExpiration);
        }
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
        _cache.Remove($"Authors_{id}");
        _cache.Remove("Authors");
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
        _cache.Remove($"Authors_{id}");
        _cache.Remove("Authors");

        return Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<Author>> Author([FromBody] Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        _cache.Remove("Authors");
        return CreatedAtAction(nameof(Author), new {id=author.Id }, author);
    }

}